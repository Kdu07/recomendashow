import { fetchCategories } from '../services/categoryService';
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

describe('categoryService', () => {
  beforeEach(() => {
    jest.clearAllMocks();
  });

  describe('fetchCategories', () => {
    test('calls the genero endpoint', async () => {
      apiFetch.mockResolvedValue(okJson([]));

      await fetchCategories();

      expect(apiFetch).toHaveBeenCalledWith('/api/Genero');
    });

    test('always prepends an "all" category', async () => {
      apiFetch.mockResolvedValue(okJson([]));

      const categories = await fetchCategories();

      expect(categories[0]).toEqual({ id: 'all', label: 'Todos' });
    });

    test('maps each genre string to an id/label pair, lowercasing the id', async () => {
      apiFetch.mockResolvedValue(okJson(['Musical', 'Drama', 'Comédia']));

      const categories = await fetchCategories();

      expect(categories).toEqual([
        { id: 'all', label: 'Todos' },
        { id: 'musical', label: 'Musical' },
        { id: 'drama', label: 'Drama' },
        { id: 'comédia', label: 'Comédia' },
      ]);
    });

    test('throws when the response is not ok', async () => {
      apiFetch.mockResolvedValue({ ok: false });

      await expect(fetchCategories()).rejects.toThrow('Não foi possível carregar as categorias');
    });
  });
});