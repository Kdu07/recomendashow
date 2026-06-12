import {
  formatCep,
  lookupAddressByCep,
  onlyCepDigits,
} from '../services/viaCepService';

import {
  cacheSize,
  flushCache,
  getLastCacheStatus,
} from '../services/vhsService';

function okJson(data) {
  return {
    ok: true,
    json: jest.fn().mockResolvedValue(data),
  };
}

describe('viaCepService', () => {
  beforeEach(() => {
    flushCache();
    global.fetch = jest.fn();
  });

  afterEach(() => {
    flushCache();
    jest.restoreAllMocks();
  });

  describe('onlyCepDigits', () => {
    test('returns an empty string for nullish values', () => {
      expect(onlyCepDigits(null)).toBe('');
      expect(onlyCepDigits(undefined)).toBe('');
    });

    test('keeps only digits and limits the value to 8 characters', () => {
      expect(onlyCepDigits('01310-100')).toBe('01310100');
      expect(onlyCepDigits('CEP 01310-100 SP')).toBe('01310100');
      expect(onlyCepDigits('1234567890')).toBe('12345678');
    });
  });

  describe('formatCep', () => {
    test('does not add a hyphen while the value has up to 5 digits', () => {
      expect(formatCep('')).toBe('');
      expect(formatCep('01310')).toBe('01310');
    });

    test('formats a complete CEP with a hyphen', () => {
      expect(formatCep('01310100')).toBe('01310-100');
      expect(formatCep('01310-100')).toBe('01310-100');
      expect(formatCep('CEP 01310100')).toBe('01310-100');
    });
  });

  describe('lookupAddressByCep', () => {
    test('rejects malformed CEPs before calling the external API', async () => {
      await expect(lookupAddressByCep('123')).rejects.toThrow(/8/);

      expect(global.fetch).not.toHaveBeenCalled();
      expect(cacheSize()).toBe(0);
      expect(getLastCacheStatus()).toBeNull();
    });

    test('calls ViaCEP with the sanitized 8-digit CEP', async () => {
      global.fetch.mockResolvedValue(okJson({
        cep: '01310-100',
        logradouro: 'Avenida Paulista',
        bairro: 'Bela Vista',
        localidade: 'Sao Paulo',
        uf: 'SP',
      }));

      await lookupAddressByCep('01310-100');

      expect(global.fetch).toHaveBeenCalledTimes(1);
      expect(global.fetch).toHaveBeenCalledWith(
        'https://viacep.com.br/ws/01310100/json/'
      );
      expect(getLastCacheStatus()).toBe('MISS');
    });

    test('maps the ViaCEP response to the app address contract', async () => {
      global.fetch.mockResolvedValue(okJson({
        cep: '01310-100',
        logradouro: 'Avenida Paulista',
        bairro: 'Bela Vista',
        localidade: 'Sao Paulo',
        uf: 'SP',
      }));

      await expect(lookupAddressByCep('01310100')).resolves.toEqual({
        cep: '01310-100',
        street: 'Avenida Paulista',
        neighborhood: 'Bela Vista',
        city: 'Sao Paulo',
        state: 'SP',
      });
    });

    test('uses a formatted fallback CEP and empty strings for missing optional fields', async () => {
      global.fetch.mockResolvedValue(okJson({
        localidade: 'Rio de Janeiro',
        uf: 'RJ',
      }));

      await expect(lookupAddressByCep('22041001')).resolves.toEqual({
        cep: '22041-001',
        street: '',
        neighborhood: '',
        city: 'Rio de Janeiro',
        state: 'RJ',
      });
    });

    test('maps a sparse ViaCEP payload to empty address fields', async () => {
      global.fetch.mockResolvedValue(okJson({}));

      await expect(lookupAddressByCep('30140071')).resolves.toEqual({
        cep: '30140-071',
        street: '',
        neighborhood: '',
        city: '',
        state: '',
      });
    });

    test('reuses the cache for equivalent raw and formatted CEP inputs', async () => {
      global.fetch.mockResolvedValue(okJson({
        cep: '01310-100',
        logradouro: 'Avenida Paulista',
        bairro: 'Bela Vista',
        localidade: 'Sao Paulo',
        uf: 'SP',
      }));

      await lookupAddressByCep('01310100');
      await lookupAddressByCep('01310-100');

      expect(global.fetch).toHaveBeenCalledTimes(1);
      expect(cacheSize()).toBe(1);
      expect(getLastCacheStatus()).toBe('HIT');
    });

    test('treats ViaCEP erro=true as CEP not found and keeps the cached not-found response', async () => {
      global.fetch.mockResolvedValue(okJson({ erro: true }));

      await expect(lookupAddressByCep('00000000')).rejects.toThrow(/encontrado/);
      await expect(lookupAddressByCep('00000-000')).rejects.toThrow(/encontrado/);

      expect(global.fetch).toHaveBeenCalledTimes(1);
      expect(cacheSize()).toBe(1);
      expect(getLastCacheStatus()).toBe('HIT');
    });

    test('does not cache HTTP errors from ViaCEP', async () => {
      global.fetch.mockResolvedValue({ ok: false });

      await expect(lookupAddressByCep('01310100')).rejects.toThrow(/consultar o CEP agora/);

      expect(global.fetch).toHaveBeenCalledTimes(1);
      expect(cacheSize()).toBe(0);
      expect(getLastCacheStatus()).toBe('MISS');
    });

    test('does not cache fetch rejections', async () => {
      global.fetch.mockRejectedValue(new Error('network down'));

      await expect(lookupAddressByCep('01310100')).rejects.toThrow(/consultar o CEP agora/);

      expect(global.fetch).toHaveBeenCalledTimes(1);
      expect(cacheSize()).toBe(0);
      expect(getLastCacheStatus()).toBe('MISS');
    });

    test('does not cache invalid JSON responses', async () => {
      global.fetch.mockResolvedValue({
        ok: true,
        json: jest.fn().mockRejectedValue(new Error('invalid json')),
      });

      await expect(lookupAddressByCep('01310100')).rejects.toThrow(/consultar o CEP agora/);

      expect(global.fetch).toHaveBeenCalledTimes(1);
      expect(cacheSize()).toBe(0);
      expect(getLastCacheStatus()).toBe('MISS');
    });
  });
});
