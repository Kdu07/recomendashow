import { fetchRecommendations } from '../services/recommendationService';
import { apiFetch } from '../services/apiClient';
import { mapDtoToHomeEvent } from '../services/showService';

jest.mock('../services/apiClient', () => ({
  apiFetch: jest.fn(),
  API_BASE: 'http://api.test',
}));

jest.mock('../services/showService', () => ({
  mapDtoToHomeEvent: jest.fn(),
}));

function okJson(data) {
  return {
    ok: true,
    json: jest.fn().mockResolvedValue(data),
  };
}

describe('recommendationService', () => {
  beforeEach(() => {
    jest.clearAllMocks();
  });

  describe('fetchRecommendations', () => {
    test('returns an empty array without calling the API when userId is falsy', async () => {
      await expect(fetchRecommendations(null)).resolves.toEqual([]);
      await expect(fetchRecommendations(undefined)).resolves.toEqual([]);
      await expect(fetchRecommendations('')).resolves.toEqual([]);

      expect(apiFetch).not.toHaveBeenCalled();
    });

    test('calls the recomendacao endpoint with a fixed limit of 5', async () => {
      apiFetch.mockResolvedValue(okJson([]));

      await fetchRecommendations('user-1');

      expect(apiFetch).toHaveBeenCalledWith('/api/Recomendacao/user-1?limit=5');
    });

    test('returns an empty array and warns when the response is not ok', async () => {
      jest.spyOn(console, 'warn').mockImplementation(() => {});
      apiFetch.mockResolvedValue({ ok: false });

      await expect(fetchRecommendations('user-1')).resolves.toEqual([]);
      expect(console.warn).toHaveBeenCalled();

      console.warn.mockRestore();
    });

    test('maps each recommendation using PascalCase Espetaculo/Score/Motivo fields', async () => {
      mapDtoToHomeEvent.mockReturnValue({ id: '1', title: 'Mamma Mia!' });
      apiFetch.mockResolvedValue(
        okJson([{ Espetaculo: { Id: '1' }, Score: 0.9, Motivo: 'Baseado nos seus gêneros favoritos' }])
      );

      const result = await fetchRecommendations('user-1');

      expect(mapDtoToHomeEvent).toHaveBeenCalledWith({ Id: '1' });
      expect(result).toEqual([
        {
          id: '1',
          title: 'Mamma Mia!',
          recommendationReason: 'Baseado nos seus gêneros favoritos',
          recommendationScore: 0.9,
        },
      ]);
    });

    test('maps each recommendation using camelCase espetaculo/score/motivo fields as a fallback', async () => {
      mapDtoToHomeEvent.mockReturnValue({ id: '2', title: 'Hamlet' });
      apiFetch.mockResolvedValue(
        okJson([{ espetaculo: { id: '2' }, score: 0.5, motivo: 'Baseado no seu histórico' }])
      );

      const result = await fetchRecommendations('user-1');

      expect(mapDtoToHomeEvent).toHaveBeenCalledWith({ id: '2' });
      expect(result).toEqual([
        {
          id: '2',
          title: 'Hamlet',
          recommendationReason: 'Baseado no seu histórico',
          recommendationScore: 0.5,
        },
      ]);
    });

    test('returns one mapped entry per recommendation in the response', async () => {
      mapDtoToHomeEvent.mockReturnValueOnce({ id: '1' }).mockReturnValueOnce({ id: '2' });
      apiFetch.mockResolvedValue(
        okJson([
          { Espetaculo: { Id: '1' }, Score: 0.9 },
          { Espetaculo: { Id: '2' }, Score: 0.8 },
        ])
      );

      const result = await fetchRecommendations('user-1');

      expect(result).toHaveLength(2);
    });
  });
});