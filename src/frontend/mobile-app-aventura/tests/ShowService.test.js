import {
  fetchShows,
  fetchShowById,
  searchShows,
  mapDtoToHomeEvent,
  mapDtoToShowInfo,
} from '../services/showService';
import { apiFetch, API_BASE } from '../services/apiClient';

jest.mock('../services/apiClient', () => ({
  apiFetch: jest.fn(),
  API_BASE: 'http://api.test',
}));

function okJson(data) {
  return {
    ok: true,
    json: jest.fn().mockResolvedValue(data),
  };
}

describe('showService', () => {
  beforeEach(() => {
    jest.clearAllMocks();
  });

  describe('fetchShows', () => {
    test('calls the espetaculo endpoint and returns the parsed body', async () => {
      const dto = [{ id: '1', nome: 'Mamma Mia!' }];
      apiFetch.mockResolvedValue(okJson(dto));

      await expect(fetchShows()).resolves.toEqual(dto);
      expect(apiFetch).toHaveBeenCalledWith('/api/Espetaculo');
    });

    test('throws when the response is not ok', async () => {
      apiFetch.mockResolvedValue({ ok: false });

      await expect(fetchShows()).rejects.toThrow('Nao foi possivel carregar os espetaculos');
    });
  });

  describe('fetchShowById', () => {
    test('calls the espetaculo endpoint with the given id', async () => {
      apiFetch.mockResolvedValue(okJson({ id: '1' }));

      await fetchShowById('1');

      expect(apiFetch).toHaveBeenCalledWith('/api/Espetaculo/1');
    });

    test('throws when the response is not ok', async () => {
      apiFetch.mockResolvedValue({ ok: false });

      await expect(fetchShowById('1')).rejects.toThrow('Espetaculo nao encontrado');
    });
  });

  describe('searchShows', () => {
    test('calls the search endpoint without query params when nothing is provided', async () => {
      apiFetch.mockResolvedValue(okJson([]));

      await searchShows(undefined, undefined);

      expect(apiFetch).toHaveBeenCalledWith('/api/Espetaculo/search');
    });

    test('includes nome in the query string when provided', async () => {
      apiFetch.mockResolvedValue(okJson([]));

      await searchShows('Hamlet', undefined);

      expect(apiFetch).toHaveBeenCalledWith('/api/Espetaculo/search?nome=Hamlet');
    });

    test('includes genero in the query string when provided and not "all"', async () => {
      apiFetch.mockResolvedValue(okJson([]));

      await searchShows(undefined, 'Musical');

      expect(apiFetch).toHaveBeenCalledWith('/api/Espetaculo/search?genero=Musical');
    });

    test('omits genero from the query string when it is "all"', async () => {
      apiFetch.mockResolvedValue(okJson([]));

      await searchShows('Hamlet', 'all');

      expect(apiFetch).toHaveBeenCalledWith('/api/Espetaculo/search?nome=Hamlet');
    });

    test('combines nome and genero in the query string when both are provided', async () => {
      apiFetch.mockResolvedValue(okJson([]));

      await searchShows('Hamlet', 'Drama');

      expect(apiFetch).toHaveBeenCalledWith('/api/Espetaculo/search?nome=Hamlet&genero=Drama');
    });

    test('throws when the response is not ok', async () => {
      apiFetch.mockResolvedValue({ ok: false });

      await expect(searchShows('Hamlet')).rejects.toThrow('Erro ao buscar espetaculos');
    });
  });

  describe('mapDtoToHomeEvent', () => {
    test('reads PascalCase fields and builds the home event shape', () => {
      const dto = { Id: '1', Nome: 'Mamma Mia!', Genero: 'Musical', TipoEvento: 'Show' };

      const event = mapDtoToHomeEvent(dto);

      expect(event.id).toBe('1');
      expect(event.showId).toBe('1');
      expect(event.title).toBe('Mamma Mia!');
      expect(event.categoryId).toBe('musical');
    });

    test('reads camelCase fields as a fallback', () => {
      const dto = { id: '2', nome: 'Hamlet', genero: 'Drama' };

      const event = mapDtoToHomeEvent(dto);

      expect(event.id).toBe('2');
      expect(event.title).toBe('Hamlet');
      expect(event.categoryId).toBe('drama');
    });

    test('prefers locationInfo.venueName for location, falling back to genero/tipoEvento', () => {
      const dto = { id: '1', nome: 'X', genero: 'Musical', tipoEvento: 'Show' };

      const withLocation = mapDtoToHomeEvent(dto, { venueName: 'Teatro Riachuelo', city: 'Rio de Janeiro' });
      expect(withLocation.location).toBe('Teatro Riachuelo');
      expect(withLocation.city).toBe('Rio de Janeiro');

      const withoutLocation = mapDtoToHomeEvent(dto, null);
      expect(withoutLocation.location).toBe('Musical');
      expect(withoutLocation.city).toBe('');
    });

    test('returns an image object pointing to the backend image endpoint when id is present', () => {
      const event = mapDtoToHomeEvent({ id: 'abc' });

      expect(event.image).toEqual({ uri: `${API_BASE}/api/Espetaculo/abc/imagem` });
    });

    test('falls back to a local image when the dto has no id and the name matches a known pattern', () => {
      const event = mapDtoToHomeEvent({ nome: 'Mamma Mia!' });

      expect(event.image).toBe('test-file-stub');
    });

    test('falls back to the default local image when no id and no name pattern matches', () => {
      const event = mapDtoToHomeEvent({ nome: 'Espetaculo Sem Padrao Conhecido' });

      expect(event.image).toBe('test-file-stub');
    });

    test('falls back to the default local image when no id and the dto is completely empty', () => {
      const event = mapDtoToHomeEvent({});

      expect(event.image).toBe('test-file-stub');
    });

    describe('getCategoryId behaviour (via mapDtoToHomeEvent.categoryId)', () => {
      test.each([
        ['Musical', 'musical'],
        ['Drama', 'drama'],
        ['Infantil', 'infantil'],
        ['Ballet', 'ballet'],
      ])('maps genero "%s" to categoryId "%s"', (genero, expected) => {
        expect(mapDtoToHomeEvent({ id: '1', genero }).categoryId).toBe(expected);
      });

      test('falls back to tipoEvento when genero is absent', () => {
        expect(mapDtoToHomeEvent({ id: '1', tipoEvento: 'Drama' }).categoryId).toBe('drama');
      });

      test('returns "all" when genero/tipoEvento do not match any known category', () => {
        expect(mapDtoToHomeEvent({ id: '1', genero: 'Stand-up' }).categoryId).toBe('all');
      });

      test('returns "all" when there is no genero or tipoEvento at all', () => {
        expect(mapDtoToHomeEvent({ id: '1' }).categoryId).toBe('all');
      });

      test('maps genero "comedia" (without accent) to categoryId "comedia"', () => {
        expect(mapDtoToHomeEvent({ id: '1', genero: 'comedia' }).categoryId).toBe('comedia');
      });

      test('BUG: "Comédia" (with accent) does not map to "comedia" due to a mojibake literal in the source', () => {
        expect(mapDtoToHomeEvent({ id: '1', genero: 'Comédia' }).categoryId).toBe('all');
      });
    });
  });

  describe('mapDtoToShowInfo', () => {
    test('builds the date status for an upcoming, not-yet-running show', () => {
      const info = mapDtoToShowInfo({
        id: '1',
        nome: 'Hamlet',
        dataInicio: '2026-08-01T00:00:00',
        emCartaz: false,
      });

      expect(info.date.status).toMatch(/^A partir de/);
    });

    test('builds the date status range for a show currently em cartaz', () => {
      const info = mapDtoToShowInfo({
        id: '1',
        nome: 'Hamlet',
        dataInicio: '2026-08-01T00:00:00',
        dataFim: '2026-09-01T00:00:00',
        emCartaz: true,
      });

      expect(info.date.status).toMatch(/^Em cartaz de/);
      expect(info.date.status).toContain(' a ');
    });

    test('falls back to "Em cartaz" when emCartaz is true and there is no start date', () => {
      const info = mapDtoToShowInfo({ id: '1', emCartaz: true });

      expect(info.date.status).toBe('Em cartaz');
    });

    test('falls back to "Data indisponivel" when emCartaz is false and there is no start date', () => {
      const info = mapDtoToShowInfo({ id: '1', emCartaz: false });

      expect(info.date.status).toBe('Data indisponivel');
    });

    test('formats currency using comma as the decimal separator', () => {
      const info = mapDtoToShowInfo({ id: '1', valorIngresso: 150 });

      expect(info.ticket.price).toBe('R$ 150,00');
    });

    test('returns an empty ticket price when valorIngresso is null or undefined', () => {
      expect(mapDtoToShowInfo({ id: '1', valorIngresso: null }).ticket.price).toBe('');
      expect(mapDtoToShowInfo({ id: '1' }).ticket.price).toBe('');
    });

    test('uses tipoEvento, falling back to genero, as the venue label', () => {
      expect(mapDtoToShowInfo({ id: '1', tipoEvento: 'Show' }).location.venue).toBe('Show');
      expect(mapDtoToShowInfo({ id: '1', genero: 'Musical' }).location.venue).toBe('Musical');
      expect(mapDtoToShowInfo({ id: '1' }).location.venue).toBe('Espetaculo');
    });

    test('falls back to a placeholder description when descricao is missing', () => {
      expect(mapDtoToShowInfo({ id: '1' }).about.description).toBe('Descricao nao disponivel.');
    });

    test('uses the provided descricao when present', () => {
      expect(mapDtoToShowInfo({ id: '1', descricao: 'Uma comédia musical' }).about.description).toBe(
        'Uma comédia musical'
      );
    });

    test('exposes the ticketing url for both the ticket section and the venue', () => {
      const info = mapDtoToShowInfo({ id: '1', urlBilheteria: 'https://bilheteria.com/x' });

      expect(info.ticket.url).toBe('https://bilheteria.com/x');
      expect(info.location.venueUrl).toBe('https://bilheteria.com/x');
    });

    test('starts as not favorited and with empty creative team and recommendations', () => {
      const info = mapDtoToShowInfo({ id: '1' });

      expect(info.isFavorited).toBe(false);
      expect(info.creativeTeam.members).toEqual([]);
      expect(info.recommendations).toEqual([]);
    });
  });
});