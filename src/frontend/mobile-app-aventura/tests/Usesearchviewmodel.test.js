/**
 * @file useSearchViewModel.test.js
 * Testes unitários para useSearchViewModel.
 */

import { renderHook, act, waitFor } from '@testing-library/react';

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
  getSearchData: jest.fn().mockResolvedValue({
    events: MOCK_EVENTS,
    categories: MOCK_CATEGORIES,
    recommendedEvents: MOCK_RECOMMENDED,
  }),
  searchEventsData: jest.fn((query, category) => {
    const normalizedQuery = String(query ?? '').trim().toLowerCase();
    return Promise.resolve(
      MOCK_EVENTS.filter((event) => {
        const matchesQuery =
          normalizedQuery.length === 0 ||
          event.title.toLowerCase().includes(normalizedQuery) ||
          event.location.toLowerCase().includes(normalizedQuery) ||
          event.city.toLowerCase().includes(normalizedQuery);
        const matchesCategory = !category || event.categoryId === category;
        return matchesQuery && matchesCategory;
      })
    );
  }),
}));

jest.mock('../repositories/wishlistRepository', () => ({
  getWishlistData: jest.fn().mockResolvedValue({ events: [] }),
  toggleWishlistEvent: jest.fn(),
}));

jest.mock('../services/tokenStorageService', () => ({
  getAuthSession: jest.fn().mockResolvedValue({ usuarioId: 'user-123' }),
}));

jest.mock('@react-navigation/native', () => ({
  useFocusEffect: jest.fn(),
}));

import { toggleWishlistEvent } from '../repositories/wishlistRepository';

const useSearchViewModel = require('../viewmodels/useSearchViewModel').default;

function makeNavigation() {
  return { navigate: jest.fn() };
}

async function renderLoadedHook(navigation = makeNavigation()) {
  const hook = renderHook(() => useSearchViewModel(navigation));
  await waitFor(() => {
    expect(hook.result.current.filteredEvents).toEqual(MOCK_RECOMMENDED);
  });
  return hook;
}

async function applySearch(result, value) {
  act(() => result.current.handleSearchChange(value));
  await waitFor(() => {
    expect(result.current.isSearching).toBe(true);
  });
}

describe('useSearchViewModel - estado inicial', () => {
  test('searchText vazio e categoria "all"', async () => {
    const { result } = await renderLoadedHook();
    expect(result.current.searchText).toBe('');
    expect(result.current.selectedCategory).toBe('all');
  });

  test('isSearching é false no estado inicial', async () => {
    const { result } = await renderLoadedHook();
    expect(result.current.isSearching).toBe(false);
  });

  test('exibe eventos recomendados quando não há busca ativa', async () => {
    const { result } = await renderLoadedHook();
    expect(result.current.filteredEvents).toEqual(MOCK_RECOMMENDED);
  });

  test('sectionTitle padrão é "Recomendado para você"', async () => {
    const { result } = await renderLoadedHook();
    expect(result.current.sectionTitle).toBe('Recomendado para você');
  });
});

describe('useSearchViewModel - busca por texto', () => {
  test('filtrar por título retorna evento correto', async () => {
    const { result } = await renderLoadedHook();
    await applySearch(result, 'Mamma');
    await waitFor(() => expect(result.current.filteredEvents).toHaveLength(1));
    expect(result.current.filteredEvents[0].id).toBe('1');
  });

  test('busca é case-insensitive', async () => {
    const { result } = await renderLoadedHook();
    await applySearch(result, 'mamma mia');
    await waitFor(() => expect(result.current.filteredEvents).toHaveLength(1));
    expect(result.current.filteredEvents[0].title).toBe('Mamma Mia');
  });

  test('filtrar por localização funciona', async () => {
    const { result } = await renderLoadedHook();
    await applySearch(result, 'Pinheiros');
    await waitFor(() => expect(result.current.filteredEvents).toHaveLength(1));
    expect(result.current.filteredEvents[0].id).toBe('2');
  });

  test('filtrar por cidade funciona', async () => {
    const { result } = await renderLoadedHook();
    await applySearch(result, 'Rio de Janeiro');
    await waitFor(() => expect(result.current.filteredEvents).toHaveLength(1));
    expect(result.current.filteredEvents[0].id).toBe('3');
  });

  test('busca sem resultado define hasResults como false', async () => {
    const { result } = await renderLoadedHook();
    await applySearch(result, 'xyzabc');
    await waitFor(() => expect(result.current.hasResults).toBe(false));
  });

  test('sectionTitle muda quando há busca ativa', async () => {
    const { result } = await renderLoadedHook();
    await applySearch(result, 'Mamma');
    expect(result.current.sectionTitle).toBe('Resultados da busca');
  });

  test('sectionSubtitle mostra contagem de resultados', async () => {
    const { result } = await renderLoadedHook();
    await applySearch(result, 'São Paulo');
    await waitFor(() => expect(result.current.filteredEvents).toHaveLength(2));
    expect(result.current.sectionSubtitle).toContain('espetáculos encontrados');
  });
});

describe('useSearchViewModel - filtro por categoria', () => {
  test('selecionar categoria "musical" filtra apenas musicais', async () => {
    const { result } = await renderLoadedHook();
    act(() => result.current.handleCategoryPress('musical'));
    await waitFor(() => expect(result.current.filteredEvents.length).toBeGreaterThan(0));
    expect(result.current.filteredEvents.every((event) => event.categoryId === 'musical')).toBe(true);
  });

  test('isSearching é true ao selecionar categoria diferente de "all"', async () => {
    const { result } = await renderLoadedHook();
    act(() => result.current.handleCategoryPress('teatro'));
    expect(result.current.isSearching).toBe(true);
  });

  test('combinação de texto + categoria aplica ambos os filtros', async () => {
    const { result } = await renderLoadedHook();
    act(() => {
      result.current.handleSearchChange('São Paulo');
      result.current.handleCategoryPress('musical');
    });
    await waitFor(() => expect(result.current.filteredEvents).toHaveLength(1));
    expect(result.current.filteredEvents[0].id).toBe('1');
  });
});

describe('useSearchViewModel - handleClearSearch', () => {
  test('limpa texto e restaura categoria para "all"', async () => {
    const { result } = await renderLoadedHook();
    await applySearch(result, 'Mamma');
    act(() => result.current.handleCategoryPress('musical'));
    act(() => result.current.handleClearSearch());
    expect(result.current.searchText).toBe('');
    expect(result.current.selectedCategory).toBe('all');
    expect(result.current.isSearching).toBe(false);
  });
});

describe('useSearchViewModel - handleEventPress', () => {
  test('navega para ShowInfo quando evento tem showId', async () => {
    const nav = makeNavigation();
    const { result } = await renderLoadedHook(nav);
    act(() => result.current.handleEventPress(MOCK_EVENTS[0]));
    expect(nav.navigate).toHaveBeenCalledWith('ShowInfo', { showId: 'show-1' });
  });

  test('não navega quando evento não tem showId', async () => {
    const nav = makeNavigation();
    const { result } = await renderLoadedHook(nav);
    act(() => result.current.handleEventPress({ id: '99' }));
    expect(nav.navigate).not.toHaveBeenCalled();
  });
});

describe('useSearchViewModel - wishlist', () => {
  test('isEventFavorite retorna false para evento não salvo', async () => {
    const { result } = await renderLoadedHook();
    expect(result.current.isEventFavorite('1')).toBe(false);
  });

  test('handleFavoritePress atualiza savedEventIds', async () => {
    toggleWishlistEvent.mockResolvedValue([MOCK_EVENTS[0]]);
    const { result } = await renderLoadedHook();
    await act(async () => { await result.current.handleFavoritePress(MOCK_EVENTS[0]); });
    expect(result.current.isEventFavorite('1')).toBe(true);
  });
});
