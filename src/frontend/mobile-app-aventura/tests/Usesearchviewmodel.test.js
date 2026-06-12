/**
 * @file useSearchViewModel.test.js
 * Testes unitários para useSearchViewModel
 *
 * A busca é delegada ao searchRepository (assíncrona, com debounce de 300ms):
 * o mock de searchEventsData reproduz a filtragem sobre dados controlados e os
 * testes aguardam o resultado com waitFor.
 */

import { renderHook, act, waitFor } from '@testing-library/react';
import useSearchViewModel from '../viewmodels/useSearchViewModel';

// ── Mocks ─────────────────────────────────────────────────────────────────────

const MOCK_EVENTS = [
  { id: '1', title: 'Mamma Mia', location: 'Teatro Renault', city: 'São Paulo', categoryId: 'musical', showId: 'show-1' },
  { id: '2', title: 'Hamlet', location: 'Sesc Pinheiros', city: 'São Paulo', categoryId: 'teatro', showId: 'show-2' },
  { id: '3', title: 'Ballet Clássico', location: 'Teatro Municipal', city: 'Rio de Janeiro', categoryId: 'danca', showId: 'show-3' },
];

const MOCK_RECOMMENDED = [
  { id: '4', title: 'Hair', location: 'Renault', city: 'São Paulo', categoryId: 'musical' },
];

const MOCK_CATEGORIES = [
  { id: 'all', label: 'Todos' },
  { id: 'musical', label: 'Musical' },
  { id: 'teatro', label: 'Teatro' },
];

jest.mock('../repositories/searchRepository', () => ({
  getSearchData: jest.fn(() =>
    Promise.resolve({
      categories: MOCK_CATEGORIES,
      recommendedEvents: MOCK_RECOMMENDED,
    })
  ),
  searchEventsData: jest.fn((text, categoryId, locationId) => {
    const term = (text || '').toLowerCase();
    return Promise.resolve(
      MOCK_EVENTS.filter((event) => {
        const matchesText =
          !term ||
          event.title.toLowerCase().includes(term) ||
          event.location.toLowerCase().includes(term) ||
          event.city.toLowerCase().includes(term);
        const matchesCategory = !categoryId || event.categoryId === categoryId;
        const matchesLocation = !locationId || event.location === locationId;
        return matchesText && matchesCategory && matchesLocation;
      })
    );
  }),
}));

jest.mock('../repositories/wishlistRepository', () => ({
  getWishlistData: jest.fn().mockResolvedValue({ events: [] }),
  toggleWishlistEvent: jest.fn(),
}));

jest.mock('../services/tokenStorageService', () => ({
  getAuthSession: jest.fn().mockResolvedValue({ usuarioId: 'user-1' }),
}));

jest.mock('@react-navigation/native', () => ({
  useFocusEffect: jest.fn(),
}));

import { toggleWishlistEvent } from '../repositories/wishlistRepository';

function makeNavigation() {
  return { navigate: jest.fn() };
}

async function setupLoaded(navigation = makeNavigation()) {
  const utils = renderHook(() => useSearchViewModel(navigation));
  // aguarda o carregamento inicial (categorias + recomendados)
  await waitFor(() =>
    expect(utils.result.current.categories.length).toBeGreaterThan(1)
  );
  return utils;
}

// ── Estado inicial ─────────────────────────────────────────────────────────────

describe('useSearchViewModel – estado inicial', () => {
  test('searchText vazio e categoria "all"', async () => {
    const { result } = await setupLoaded();
    expect(result.current.searchText).toBe('');
    expect(result.current.selectedCategory).toBe('all');
  });

  test('isSearching é false no estado inicial', async () => {
    const { result } = await setupLoaded();
    expect(result.current.isSearching).toBe(false);
  });

  test('exibe eventos recomendados quando não há busca ativa', async () => {
    const { result } = await setupLoaded();
    expect(result.current.filteredEvents).toEqual(MOCK_RECOMMENDED);
  });

  test('sectionTitle padrão é "Recomendado para você"', async () => {
    const { result } = await setupLoaded();
    expect(result.current.sectionTitle).toBe('Recomendado para você');
  });
});

// ── Busca por texto ────────────────────────────────────────────────────────────

describe('useSearchViewModel – busca por texto', () => {
  test('filtrar por título retorna evento correto', async () => {
    const { result } = await setupLoaded();
    act(() => result.current.handleSearchChange('Mamma'));
    await waitFor(() => expect(result.current.filteredEvents).toHaveLength(1));
    expect(result.current.filteredEvents[0].id).toBe('1');
  });

  test('busca é case-insensitive', async () => {
    const { result } = await setupLoaded();
    act(() => result.current.handleSearchChange('mamma mia'));
    await waitFor(() => expect(result.current.filteredEvents).toHaveLength(1));
    expect(result.current.filteredEvents[0].title).toBe('Mamma Mia');
  });

  test('filtrar por localização funciona', async () => {
    const { result } = await setupLoaded();
    act(() => result.current.handleSearchChange('Pinheiros'));
    await waitFor(() => expect(result.current.filteredEvents).toHaveLength(1));
    expect(result.current.filteredEvents[0].id).toBe('2');
  });

  test('filtrar por cidade funciona', async () => {
    const { result } = await setupLoaded();
    act(() => result.current.handleSearchChange('Rio de Janeiro'));
    await waitFor(() => expect(result.current.filteredEvents).toHaveLength(1));
    expect(result.current.filteredEvents[0].id).toBe('3');
  });

  test('busca sem resultado define hasResults como false', async () => {
    const { result } = await setupLoaded();
    act(() => result.current.handleSearchChange('xyzabc'));
    await waitFor(() => expect(result.current.isSearching).toBe(true));
    await waitFor(() => expect(result.current.hasResults).toBe(false));
  });

  test('sectionTitle muda quando há busca ativa', async () => {
    const { result } = await setupLoaded();
    act(() => result.current.handleSearchChange('Mamma'));
    await waitFor(() => expect(result.current.sectionTitle).toBe('Resultados da busca'));
  });

  test('sectionSubtitle mostra contagem de resultados', async () => {
    const { result } = await setupLoaded();
    act(() => result.current.handleSearchChange('São Paulo'));
    // Mamma Mia + Hamlet estão em São Paulo
    await waitFor(() =>
      expect(result.current.sectionSubtitle).toContain('espetáculos encontrados')
    );
    expect(result.current.filteredEvents).toHaveLength(2);
  });
});

// ── Filtro por categoria ───────────────────────────────────────────────────────

describe('useSearchViewModel – filtro por categoria', () => {
  test('selecionar categoria "musical" filtra apenas musicais', async () => {
    const { result } = await setupLoaded();
    act(() => result.current.handleCategoryPress('musical'));
    await waitFor(() => expect(result.current.filteredEvents.length).toBeGreaterThan(0));
    expect(result.current.filteredEvents.every((e) => e.categoryId === 'musical')).toBe(true);
  });

  test('isSearching é true ao selecionar categoria diferente de "all"', async () => {
    const { result } = await setupLoaded();
    act(() => result.current.handleCategoryPress('teatro'));
    expect(result.current.isSearching).toBe(true);
  });

  test('combinação de texto + categoria aplica ambos os filtros', async () => {
    const { result } = await setupLoaded();
    act(() => {
      result.current.handleSearchChange('São Paulo');
      result.current.handleCategoryPress('musical');
    });
    // Apenas Mamma Mia é musical em São Paulo
    await waitFor(() => expect(result.current.filteredEvents).toHaveLength(1));
    expect(result.current.filteredEvents[0].id).toBe('1');
  });
});

// ── handleClearSearch ─────────────────────────────────────────────────────────

describe('useSearchViewModel – handleClearSearch', () => {
  test('limpa texto e restaura categoria para "all"', async () => {
    const { result } = await setupLoaded();
    act(() => {
      result.current.handleSearchChange('Mamma');
      result.current.handleCategoryPress('musical');
    });
    act(() => result.current.handleClearSearch());
    expect(result.current.searchText).toBe('');
    expect(result.current.selectedCategory).toBe('all');
    expect(result.current.isSearching).toBe(false);
  });
});

// ── handleEventPress ──────────────────────────────────────────────────────────

describe('useSearchViewModel – handleEventPress', () => {
  test('navega para ShowInfo quando evento tem showId', async () => {
    const nav = makeNavigation();
    const { result } = await setupLoaded(nav);
    act(() => result.current.handleEventPress(MOCK_EVENTS[0]));
    expect(nav.navigate).toHaveBeenCalledWith('ShowInfo', { showId: 'show-1' });
  });

  test('não navega quando evento não tem showId', async () => {
    const nav = makeNavigation();
    const { result } = await setupLoaded(nav);
    act(() => result.current.handleEventPress({ id: '99' }));
    expect(nav.navigate).not.toHaveBeenCalled();
  });
});

// ── Wishlist ──────────────────────────────────────────────────────────────────

describe('useSearchViewModel – wishlist', () => {
  test('isEventFavorite retorna false para evento não salvo', async () => {
    const { result } = await setupLoaded();
    expect(result.current.isEventFavorite('1')).toBe(false);
  });

  test('handleFavoritePress atualiza savedEventIds', async () => {
    toggleWishlistEvent.mockResolvedValue([MOCK_EVENTS[0]]);
    const { result } = await setupLoaded();
    await act(async () => { await result.current.handleFavoritePress(MOCK_EVENTS[0]); });
    expect(result.current.isEventFavorite('1')).toBe(true);
  });
});
