/**
 * @file theaterService.test.js
 * Testes unitários para theaterService
 */

import { fetchTheaters, fetchTheaterShows, mapDtoToTheater } from '../services/theaterService';
import { apiFetch } from '../services/apiClient';

jest.mock('../services/apiClient', () => ({
  apiFetch: jest.fn(),
}));

function okJson(data) {
  return {
    ok: true,
    json: jest.fn().mockResolvedValue(data),
  };
}

describe('theaterService', () => {
  beforeEach(() => {
    jest.clearAllMocks();
  });

  describe('fetchTheaters', () => {
    test('calls the teatro endpoint and returns the parsed body', async () => {
      const dto = [{ id: '1', nome: 'Teatro Riachuelo' }];
      apiFetch.mockResolvedValue(okJson(dto));

      await expect(fetchTheaters()).resolves.toEqual(dto);
      expect(apiFetch).toHaveBeenCalledWith('/api/Teatro');
    });

    test('throws when the response is not ok', async () => {
      apiFetch.mockResolvedValue({ ok: false });

      await expect(fetchTheaters()).rejects.toThrow('Nao foi possivel carregar os teatros');
    });
  });

  describe('fetchTheaterShows', () => {
    test('calls the espetaculos endpoint for the given theater id', async () => {
      apiFetch.mockResolvedValue(okJson([]));

      await fetchTheaterShows('theater-1');

      expect(apiFetch).toHaveBeenCalledWith('/api/Teatro/theater-1/espetaculos');
    });

    test('throws when the response is not ok', async () => {
      apiFetch.mockResolvedValue({ ok: false });

      await expect(fetchTheaterShows('theater-1')).rejects.toThrow(
        'Nao foi possivel carregar os espetaculos do teatro'
      );
    });
  });

  describe('mapDtoToTheater', () => {
    test('reads PascalCase fields from the DTO', () => {
      const dto = {
        Id: '1',
        Nome: 'Teatro Riachuelo',
        Cidade: 'Rio de Janeiro',
        Endereco: 'Rua do Passeio, 40',
        HorarioFuncionamento: '10h às 22h',
      };

      const theater = mapDtoToTheater(dto);

      expect(theater.id).toBe('1');
      expect(theater.name).toBe('Teatro Riachuelo');
      expect(theater.city).toBe('Rio de Janeiro');
      expect(theater.location).toBe('Rua do Passeio, 40');
    });

    test('reads camelCase fields from the DTO as a fallback', () => {
      const dto = {
        id: '2',
        nome: 'Teatro Municipal',
        cidade: 'São Paulo',
        endereco: 'Praça Ramos de Azevedo',
        horarioFuncionamento: '9h às 18h',
      };

      const theater = mapDtoToTheater(dto);

      expect(theater.id).toBe('2');
      expect(theater.name).toBe('Teatro Municipal');
      expect(theater.city).toBe('São Paulo');
    });

    test('builds a description from opening hours when available', () => {
      const theater = mapDtoToTheater({ nome: 'Teatro X', horarioFuncionamento: '10h às 22h' });

      expect(theater.description).toBe('Horario de funcionamento: 10h às 22h');
    });

    test('falls back to a generic description when opening hours are missing', () => {
      const theater = mapDtoToTheater({ nome: 'Teatro X' });

      expect(theater.description).toBe('Detalhes do teatro indisponiveis no momento.');
    });

    test('builds highlights using city, location and opening hours when present', () => {
      const theater = mapDtoToTheater({
        nome: 'Teatro X',
        cidade: 'Rio de Janeiro',
        endereco: 'Rua A, 1',
        horarioFuncionamento: '10h às 22h',
      });

      expect(theater.highlights).toEqual([
        'Localizado em Rio de Janeiro',
        'Rua A, 1',
        'Aberto: 10h às 22h',
      ]);
    });

    test('builds fallback highlights when city, location and opening hours are missing', () => {
      const theater = mapDtoToTheater({ nome: 'Teatro X' });

      expect(theater.highlights).toEqual([
        'Cidade nao informada',
        'Endereco nao informado',
        'Aberto ao publico',
      ]);
    });

    test('always reports a fixed capacity message', () => {
      const theater = mapDtoToTheater({ nome: 'Teatro X' });

      expect(theater.capacity).toBe('Consultar lotacao local');
    });

    test('always resolves to an image, whether or not the name matches a known theater', () => {
      expect(mapDtoToTheater({ nome: 'Teatro Desconhecido' }).image).toBe('test-file-stub');
      expect(mapDtoToTheater({ nome: 'TEATRO RIACHUELO RIO' }).image).toBe('test-file-stub');
      expect(mapDtoToTheater({}).image).toBe('test-file-stub');
    });
  });
});