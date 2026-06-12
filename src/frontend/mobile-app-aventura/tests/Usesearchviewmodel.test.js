/**
 * @file useSearchViewModel.test.js
 * Testes unitários para useSearchViewModel
 *
 * O repositório de busca e o de wishlist são mockados com dados controlados,
 * permitindo verificar a lógica de filtragem sem dependência de AsyncStorage.
 */

import { renderHook, act } from '@testing-library/react';
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
  getSearchData: () => ({
    events: MOCK_EVENTS,
    categories: MOCK_CATEGORIES,
    recommendedEvents: MOCK_RECOMMENDED,
  }),
}));

jest.mock('../repositories/wishlistRepository', () => ({
  getWishlistData: jest.fn().mockResolvedValue({ events: [] }),
  toggleWishlistEvent: jest.fn(),
}));

jest.mock('@react-navigation/native', () => ({
  useFocusEffect: jest.fn(),
}));

import { getWishlistData, toggleWishlistEvent } from '../repositories/wishlistRepository';

function makeNavigation() {
  return { navigate: jest.fn() };
}

// ── Estado inicial ─────────────────────────────────────────────────────────────

describe('useSearchViewModel – estado inicial', () => {
  test('searchText vazio e categoria "all"', () => {
    const { result } = renderHook(() => useSearchViewModel(makeNavigation()));
    expect(result.current.searchText).toBe('');
    expect(result.current.selectedCategory).toBe('all');
  });

  test('isSearching é false no estado inicial', () => {
    const { result } = renderHook(() => useSearchViewModel(makeNavigation()));
    expect(result.current.isSearching).toBe(false);
  });

  test('exibe eventos recomendados quando não há busca ativa', () => {
    const { result } = renderHook(() => useSearchViewModel(makeNavigation()));
    expect(result.current.filteredEvents).toEqual(MOCK_RECOMMENDED);
  });

  test('sectionTitle padrão é "Recomendado para você"', () => {
    const { result } = renderHook(() => useSearchViewModel(makeNavigation()));
    expect(result.current.sectionTitle).toBe('Recomendado para você');
  });
});

// ── Busca por texto ────────────────────────────────────────────────────────────

describe('useSearchViewModel – busca por texto', () => {
  test('filtrar por título retorna evento correto', () => {
    const { result } = renderHook(() => useSearchViewModel(makeNavigation()));
    act(() => result.current.handleSearchChange('Mamma'));
    expect(result.current.filteredEvents).toHaveLength(1);
    expect(result.current.filteredEvents[0].id).toBe('1');
  });

  test('busca é case-insensitive', () => {
    const { result } = renderHook(() => useSearchViewModel(makeNavigation()));
    act(() => result.current.handleSearchChange('mamma mia'));
    expect(result.current.filteredEvents[0].title).toBe('Mamma Mia');
  });

  test('filtrar por localização funciona', () => {
    const { result } = renderHook(() => useSearchViewModel(makeNavigation()));
    act(() => result.current.handleSearchChange('Pinheiros'));
    expect(result.current.filteredEvents[0].id).toBe('2');
  });

  test('filtrar por cidade funciona', () => {
    const { result } = renderHook(() => useSearchViewModel(makeNavigation()));
    act(() => result.current.handleSearchChange('Rio de Janeiro'));
    expect(result.current.filteredEvents[0].id).toBe('3');
  });

  test('busca sem resultado define hasResults como false', () => {
    const { result } = renderHook(() => useSearchViewModel(makeNavigation()));
    act(() => result.current.handleSearchChange('xyzabc'));
    expect(result.current.hasResults).toBe(false);
  });

  test('sectionTitle muda quando há busca ativa', () => {
    const { result } = renderHook(() => useSearchViewModel(makeNavigation()));
    act(() => result.current.handleSearchChange('Mamma'));
    expect(result.current.sectionTitle).toBe('Resultados da busca');
  });

  test('sectionSubtitle mostra contagem de resultados', () => {
    const { result } = renderHook(() => useSearchViewModel(makeNavigation()));
    act(() => result.current.handleSearchChange('São Paulo'));
    // Mamma Mia + Hamlet estão em São Paulo
    expect(result.current.sectionSubtitle).toContain('espetáculos encontrados');
  });
});

// ── Filtro por categoria ───────────────────────────────────────────────────────

describe('useSearchViewModel – filtro por categoria', () => {
  test('selecionar categoria "musical" filtra apenas musicais', () => {
    const { result } = renderHook(() => useSearchViewModel(makeNavigation()));
    act(() => result.current.handleCategoryPress('musical'));
    expect(result.current.filteredEvents.every((e) => e.categoryId === 'musical')).toBe(true);
  });

  test('isSearching é true ao selecionar categoria diferente de "all"', () => {
    const { result } = renderHook(() => useSearchViewModel(makeNavigation()));
    act(() => result.current.handleCategoryPress('teatro'));
    expect(result.current.isSearching).toBe(true);
  });

  test('combinação de texto + categoria aplica ambos os filtros', () => {
    const { result } = renderHook(() => useSearchViewModel(makeNavigation()));
    act(() => {
      result.current.handleSearchChange('São Paulo');
      result.current.handleCategoryPress('musical');
    });
    // Apenas Mamma Mia é musical em São Paulo
    expect(result.current.filteredEvents).toHaveLength(1);
    expect(result.current.filteredEvents[0].id).toBe('1');
  });
});

// ── handleClearSearch ─────────────────────────────────────────────────────────

describe('useSearchViewModel – handleClearSearch', () => {
  test('limpa texto e restaura categoria para "all"', () => {
    const { result } = renderHook(() => useSearchViewModel(makeNavigation()));
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
  test('navega para ShowInfo quando evento tem showId', () => {
    const nav = makeNavigation();
    const { result } = renderHook(() => useSearchViewModel(nav));
    act(() => result.current.handleEventPress(MOCK_EVENTS[0]));
    expect(nav.navigate).toHaveBeenCalledWith('ShowInfo', { showId: 'show-1' });
  });

  test('não navega quando evento não tem showId', () => {
    const nav = makeNavigation();
    const { result } = renderHook(() => useSearchViewModel(nav));
    act(() => result.current.handleEventPress({ id: '99' }));
    expect(nav.navigate).not.toHaveBeenCalled();
  });
});

// ── Wishlist ──────────────────────────────────────────────────────────────────

describe('useSearchViewModel – wishlist', () => {
  test('isEventFavorite retorna false para evento não salvo', () => {
    const { result } = renderHook(() => useSearchViewModel(makeNavigation()));
    expect(result.current.isEventFavorite('1')).toBe(false);
  });

  test('handleFavoritePress atualiza savedEventIds', async () => {
    toggleWishlistEvent.mockResolvedValue([MOCK_EVENTS[0]]);
    const { result } = renderHook(() => useSearchViewModel(makeNavigation()));
    await act(async () => { await result.current.handleFavoritePress(MOCK_EVENTS[0]); });
    expect(result.current.isEventFavorite('1')).toBe(true);
  });
});
