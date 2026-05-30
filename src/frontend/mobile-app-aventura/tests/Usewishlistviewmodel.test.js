import { renderHook, act } from '@testing-library/react';
import useWishlistViewModel from '../viewmodels/useWishlistViewModel';

const MOCK_EVENTS = [
  { id: 'e1', title: 'Mamma Mia', location: 'Teatro Renault', city: 'São Paulo', showId: 'show-1' },
  { id: 'e2', title: 'Hamlet', location: 'Sesc', city: 'São Paulo', showId: 'show-2' },
  { id: 'e3', title: 'Ballet', location: 'Municipal', city: 'Rio de Janeiro', showId: 'show-3' },
];

jest.mock('../repositories/wishlistRepository', () => ({
  getWishlistData: jest.fn(),
  removeWishlistEvent: jest.fn(),
}));

jest.mock('@react-navigation/native', () => ({
  useFocusEffect: jest.fn(),
}));

import { getWishlistData, removeWishlistEvent } from '../repositories/wishlistRepository';

function makeNavigation() {
  return { navigate: jest.fn() };
}

// helper: renderiza o hook já com o carregamento inicial completo
async function setup(nav) {
  let result;
  await act(async () => {
    result = renderHook(() => useWishlistViewModel(nav ?? makeNavigation()));
  });
  return result;
}

beforeEach(() => {
  getWishlistData.mockResolvedValue({ events: [...MOCK_EVENTS] });
  removeWishlistEvent.mockResolvedValue([MOCK_EVENTS[1], MOCK_EVENTS[2]]);
});

// ── Carregamento ──────────────────────────────────────────────────────────────

describe('useWishlistViewModel – carregamento', () => {
  test('isLoading vai para false após carregar', async () => {
    const { result } = await setup();
    expect(result.current.isLoading).toBe(false);
  });

  test('savedEvents é preenchido com dados do repositório', async () => {
    const { result } = await setup();
    expect(result.current.savedEvents).toHaveLength(3);
  });

  test('hasSavedEvents é true quando há eventos', async () => {
    const { result } = await setup();
    expect(result.current.hasSavedEvents).toBe(true);
  });

  test('hasSavedEvents é false quando wishlist está vazia', async () => {
    getWishlistData.mockResolvedValue({ events: [] });
    const { result } = await setup();
    expect(result.current.hasSavedEvents).toBe(false);
  });
});

// ── Filtro por texto ──────────────────────────────────────────────────────────

describe('useWishlistViewModel – filtro por texto', () => {
  test('sem texto retorna todos os eventos salvos', async () => {
    const { result } = await setup();
    expect(result.current.filteredWishlistEvents).toHaveLength(3);
  });

  test('filtrar por título funciona', async () => {
    const { result } = await setup();
    act(() => result.current.handleSearchChange('Mamma'));
    expect(result.current.filteredWishlistEvents).toHaveLength(1);
    expect(result.current.filteredWishlistEvents[0].id).toBe('e1');
  });

  test('filtrar por localização funciona', async () => {
    const { result } = await setup();
    act(() => result.current.handleSearchChange('Sesc'));
    expect(result.current.filteredWishlistEvents[0].id).toBe('e2');
  });

  test('filtrar por cidade funciona', async () => {
    const { result } = await setup();
    act(() => result.current.handleSearchChange('Rio'));
    expect(result.current.filteredWishlistEvents[0].id).toBe('e3');
  });

  test('filtro é case-insensitive', async () => {
    const { result } = await setup();
    act(() => result.current.handleSearchChange('mamma mia'));
    expect(result.current.filteredWishlistEvents).toHaveLength(1);
  });

  test('hasFilteredResults é false quando nenhum evento passa pelo filtro', async () => {
    const { result } = await setup();
    act(() => result.current.handleSearchChange('xyzxyz'));
    expect(result.current.hasFilteredResults).toBe(false);
  });
});

// ── Remoção ───────────────────────────────────────────────────────────────────

describe('useWishlistViewModel – handleRemoveFromWishlist', () => {
  test('remove evento da lista após chamar o repositório', async () => {
    removeWishlistEvent.mockResolvedValue([MOCK_EVENTS[1], MOCK_EVENTS[2]]);
    const { result } = await setup();
    await act(async () => { await result.current.handleRemoveFromWishlist('e1'); });
    expect(result.current.savedEvents).toHaveLength(2);
    expect(result.current.savedEvents.find((e) => e.id === 'e1')).toBeUndefined();
  });

  test('chama removeWishlistEvent com o id correto', async () => {
    const { result } = await setup();
    await act(async () => { await result.current.handleRemoveFromWishlist('e2'); });
    expect(removeWishlistEvent).toHaveBeenCalledWith('e2');
  });
});

// ── Navegação ─────────────────────────────────────────────────────────────────

describe('useWishlistViewModel – handleEventPress', () => {
  test('navega para ShowInfo quando evento tem showId', async () => {
    const nav = makeNavigation();
    const { result } = await setup(nav);
    act(() => result.current.handleEventPress(MOCK_EVENTS[0]));
    expect(nav.navigate).toHaveBeenCalledWith('ShowInfo', { showId: 'show-1' });
  });

  test('não navega quando evento não tem showId', async () => {
    const nav = makeNavigation();
    const { result } = await setup(nav);
    act(() => result.current.handleEventPress({ id: '99' }));
    expect(nav.navigate).not.toHaveBeenCalled();
  });
});