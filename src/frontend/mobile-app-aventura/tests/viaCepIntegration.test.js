import {
  lookupAddressByCep,
} from '../services/viaCepService';

import {
  flushCache,
  cacheSize,
  getLastCacheStatus,
} from '../services/vhsService';

describe('TI-04 - ViaCEP + VHS', () => {

  beforeEach(() => {
    flushCache();
    global.fetch = jest.fn();
  });

  afterEach(() => {
    jest.clearAllMocks();
    flushCache();
  });

  test('deve consultar ViaCEP e armazenar resposta', async () => {

    global.fetch.mockResolvedValue({
      ok: true,
      json: async () => ({
        cep: '01310-100',
        logradouro: 'Avenida Paulista',
        bairro: 'Bela Vista',
        localidade: 'São Paulo',
        uf: 'SP',
      }),
    });

    const result =
      await lookupAddressByCep('01310100');

    expect(result.city).toBe('São Paulo');

    expect(getLastCacheStatus())
      .toBe('MISS');

    expect(cacheSize())
      .toBe(1);

    expect(global.fetch)
      .toHaveBeenCalledTimes(1);
  });

  test('deve retornar cache HIT na segunda chamada', async () => {

    global.fetch.mockResolvedValue({
      ok: true,
      json: async () => ({
        cep: '01310-100',
        logradouro: 'Avenida Paulista',
        bairro: 'Bela Vista',
        localidade: 'São Paulo',
        uf: 'SP',
      }),
    });

    await lookupAddressByCep('01310100');

    await lookupAddressByCep('01310100');

    expect(getLastCacheStatus())
      .toBe('HIT');

    expect(global.fetch)
      .toHaveBeenCalledTimes(1);
  });

  test('deve retornar CEP não encontrado', async () => {

    global.fetch.mockResolvedValue({
      ok: true,
      json: async () => ({
        erro: true,
      }),
    });

    await expect(
      lookupAddressByCep('00000000')
    ).rejects.toThrow(
      'CEP não encontrado.'
    );

    expect(getLastCacheStatus())
      .toBe('MISS');
  });

  test('deve retornar erro quando serviço estiver indisponível', async () => {

    global.fetch.mockResolvedValue({
      ok: false,
    });

    await expect(
      lookupAddressByCep('01310100')
    ).rejects.toThrow(
      'Não foi possível consultar o CEP agora.'
    );

    expect(cacheSize())
      .toBe(0);
  });

  test('não deve chamar API para CEP inválido', async () => {

    await expect(
      lookupAddressByCep('123')
    ).rejects.toThrow(
      'Informe um CEP válido com 8 dígitos.'
    );

    expect(global.fetch)
      .not.toHaveBeenCalled();
  });

});