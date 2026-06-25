import { renderHook, act, waitFor } from '@testing-library/react';
import { useFocusEffect } from '@react-navigation/native';

import useHomeViewModel from '../viewmodels/useHomeViewModel';
import { getHomeData } from '../repositories/homeRepository';
import { getWishlistData, toggleWishlistEvent } from '../repositories/wishlistRepository';
import { getAuthSession } from '../services/tokenStorageService';
import { fetchPreferences } from '../services/preferenceService';
import { getUserInfo } from '../services/userProfileService';

jest.mock('@react-navigation/native', () => ({
  useFocusEffect: jest.fn(),
}));

jest.mock('../repositories/homeRepository', () => ({
  getHomeData: jest.fn(),
}));

jest.mock('../repositories/wishlistRepository', () => ({
  getWishlistData: jest.fn(),
  toggleWishlistEvent: jest.fn(),
}));

jest.mock('../services/tokenStorageService', () => ({
  getAuthSession: jest.fn(),
}));

jest.mock('../services/preferenceService', () => ({
  fetchPreferences: jest.fn(),
}));

jest.mock('../services/userProfileService', () => ({
  getUserInfo: jest.fn(),
}));

const EMPTY_HOME_DATA = {
  categories: [{ id: 'all', label: 'Todos' }],
  heroBanners: [],
  theaters: [],
  trendingEvents: [],
  recommendedEvents: [],
  newEvents: [],
  nearbyEvents: [],
};

const MUSICAL_EVENT = { id: '1', showId: '1', title: 'Mamma Mia!', categoryId: 'musical' };
const DRAMA_EVENT = { id: '2', showId: '2', title: 'Hamlet', categoryId: 'drama' };

function createNavigation() {
  return {
    navigate: jest.fn(),
    reset: jest.fn(),
  };
}

describe('useHomeViewModel', () => {
  beforeEach(() => {
    jest.clearAllMocks();
    jest.spyOn(console, 'error').mockImplementation(() => {});
    jest.spyOn(console, 'log').mockImplementation(() => {});

    getAuthSession.mockResolvedValue(null);
    getHomeData.mockResolvedValue(EMPTY_HOME_DATA);
    getWishlistData.mockResolvedValue({ events: [] });

    // useFocusEffect normalmente só dispara quando a tela ganha foco; nos testes,
    // executamos o callback uma única vez (na "montagem"), sem reexecutar em cada
    // re-render, para refletir o comportamento real sem entrar em loop.
    let hasRunFocusEffect = false;
    useFocusEffect.mockImplementation((callback) => {
      if (!hasRunFocusEffect) {
        hasRunFocusEffect = true;
        callback();
      }
    });
  });

  afterEach(() => {
    console.error.mockRestore();
    console.log.mockRestore();
  });

  describe('initial load', () => {
    test('loads home data without a session (anonymous user)', async () => {
      const navigation = createNavigation();
      getHomeData.mockResolvedValue({
        ...EMPTY_HOME_DATA,
        trendingEvents: [MUSICAL_EVENT],
      });

      const { result } = renderHook(() => useHomeViewModel(navigation));

      await waitFor(() => expect(result.current.trendingEvents).toHaveLength(1));

      expect(getHomeData).toHaveBeenCalledWith(undefined, null);
      expect(result.current.hasInterests).toBe(false);
      expect(result.current.hasLocation).toBe(false);
    });

    test('loads preferences and profile city when a session exists', async () => {
      const navigation = createNavigation();
      getAuthSession.mockResolvedValue({ usuarioId: 'user-1' });
      fetchPreferences.mockResolvedValue({ generos: ['Musical'], cidade: 'São Paulo' });
      getUserInfo.mockResolvedValue({ city: 'Rio de Janeiro' });

      const { result } = renderHook(() => useHomeViewModel(navigation));

      await waitFor(() => expect(result.current.hasLocation).toBe(true));

      expect(result.current.hasInterests).toBe(true);
      expect(result.current.nearbyCity).toBe('São Paulo');
      expect(getHomeData).toHaveBeenCalledWith('user-1', 'São Paulo');
    });

    test('falls back to the profile city when preferences have no city', async () => {
      const navigation = createNavigation();
      getAuthSession.mockResolvedValue({ usuarioId: 'user-1' });
      fetchPreferences.mockResolvedValue({ generos: [], tags: [] });
      getUserInfo.mockResolvedValue({ city: 'Rio de Janeiro' });

      const { result } = renderHook(() => useHomeViewModel(navigation));

      await waitFor(() => expect(result.current.hasLocation).toBe(true));

      expect(result.current.nearbyCity).toBe('Rio de Janeiro');
      expect(result.current.hasInterests).toBe(false);
    });

    test('treats hasInterests as true when only tags are present (no generos)', async () => {
      const navigation = createNavigation();
      getAuthSession.mockResolvedValue({ usuarioId: 'user-1' });
      fetchPreferences.mockResolvedValue({ generos: [], tags: ['classico'] });
      getUserInfo.mockResolvedValue(null);

      const { result } = renderHook(() => useHomeViewModel(navigation));

      await waitFor(() => expect(result.current.hasInterests).toBe(true));
    });

    test('treats a failed profile fetch as no profile, without breaking the flow', async () => {
      const navigation = createNavigation();
      getAuthSession.mockResolvedValue({ usuarioId: 'user-1' });
      fetchPreferences.mockResolvedValue(null);
      getUserInfo.mockRejectedValue(new Error('network down'));

      const { result } = renderHook(() => useHomeViewModel(navigation));

      await waitFor(() => expect(getHomeData).toHaveBeenCalled());

      expect(result.current.hasLocation).toBe(false);
      expect(result.current.nearbyCity).toBe('');
    });

    test('logs an error and keeps the default empty home data when getHomeData rejects', async () => {
      const navigation = createNavigation();
      getHomeData.mockRejectedValue(new Error('server down'));

      const { result } = renderHook(() => useHomeViewModel(navigation));

      await waitFor(() => expect(console.error).toHaveBeenCalled());

      expect(result.current.heroBanners).toEqual([]);
      expect(result.current.categories).toEqual([{ id: 'all', label: 'Todos' }]);
    });
  });

  describe('category filtering', () => {
    test('shows all events when selectedCategory is "all"', async () => {
      const navigation = createNavigation();
      getHomeData.mockResolvedValue({
        ...EMPTY_HOME_DATA,
        trendingEvents: [MUSICAL_EVENT, DRAMA_EVENT],
      });

      const { result } = renderHook(() => useHomeViewModel(navigation));
      await waitFor(() => expect(result.current.trendingEvents).toHaveLength(2));
    });

    test('filters trending, recommended, new and nearby events by the selected category', async () => {
      const navigation = createNavigation();
      getHomeData.mockResolvedValue({
        ...EMPTY_HOME_DATA,
        trendingEvents: [MUSICAL_EVENT, DRAMA_EVENT],
        recommendedEvents: [MUSICAL_EVENT, DRAMA_EVENT],
        newEvents: [MUSICAL_EVENT, DRAMA_EVENT],
        nearbyEvents: [MUSICAL_EVENT, DRAMA_EVENT],
      });

      const { result } = renderHook(() => useHomeViewModel(navigation));
      await waitFor(() => expect(result.current.trendingEvents).toHaveLength(2));

      act(() => {
        result.current.handleCategoryPress('musical');
      });

      expect(result.current.selectedCategory).toBe('musical');
      expect(result.current.trendingEvents).toEqual([MUSICAL_EVENT]);
      expect(result.current.recommendedEvents).toEqual([MUSICAL_EVENT]);
      expect(result.current.newEvents).toEqual([MUSICAL_EVENT]);
      expect(result.current.nearbyEvents).toEqual([MUSICAL_EVENT]);
    });
  });

  describe('navigation handlers', () => {
    test('handleEventPress navigates to ShowInfo when the event has a showId', async () => {
      const navigation = createNavigation();
      const { result } = renderHook(() => useHomeViewModel(navigation));
      await waitFor(() => expect(getHomeData).toHaveBeenCalled());

      result.current.handleEventPress(MUSICAL_EVENT);

      expect(navigation.navigate).toHaveBeenCalledWith('ShowInfo', { showId: '1' });
    });

    test('handleEventPress does nothing when the event has no showId', async () => {
      const navigation = createNavigation();
      const { result } = renderHook(() => useHomeViewModel(navigation));
      await waitFor(() => expect(getHomeData).toHaveBeenCalled());

      result.current.handleEventPress({ id: '1' });

      expect(navigation.navigate).not.toHaveBeenCalled();
    });

    test('handleBannerPress navigates to ShowInfo when the banner has a showId', async () => {
      const navigation = createNavigation();
      const { result } = renderHook(() => useHomeViewModel(navigation));
      await waitFor(() => expect(getHomeData).toHaveBeenCalled());

      result.current.handleBannerPress({ showId: '1' });

      expect(navigation.navigate).toHaveBeenCalledWith('ShowInfo', { showId: '1' });
    });

    test('handleBannerPress does nothing when the banner has no showId', async () => {
      const navigation = createNavigation();
      const { result } = renderHook(() => useHomeViewModel(navigation));
      await waitFor(() => expect(getHomeData).toHaveBeenCalled());

      result.current.handleBannerPress({});

      expect(navigation.navigate).not.toHaveBeenCalled();
    });

    test('handleAddLocation navigates to EditProfile', async () => {
      const navigation = createNavigation();
      const { result } = renderHook(() => useHomeViewModel(navigation));
      await waitFor(() => expect(getHomeData).toHaveBeenCalled());

      result.current.handleAddLocation();

      expect(navigation.navigate).toHaveBeenCalledWith('EditProfile');
    });

    test('handleTheaterPress, handleEditInterests and handleTabPress do not throw', async () => {
      const navigation = createNavigation();
      const { result } = renderHook(() => useHomeViewModel(navigation));
      await waitFor(() => expect(getHomeData).toHaveBeenCalled());

      expect(() => result.current.handleTheaterPress({ name: 'Teatro X' })).not.toThrow();
      expect(() => result.current.handleEditInterests()).not.toThrow();
      expect(() => result.current.handleTabPress('search')).not.toThrow();
    });
  });

  describe('favorites', () => {
    test('loads saved event ids on focus', async () => {
      const navigation = createNavigation();
      getWishlistData.mockResolvedValue({ events: [MUSICAL_EVENT] });

      const { result } = renderHook(() => useHomeViewModel(navigation));

      await waitFor(() => expect(result.current.savedEventIds).toEqual(['1']));
    });

    test('isEventFavorite reflects the loaded saved event ids', async () => {
      const navigation = createNavigation();
      getWishlistData.mockResolvedValue({ events: [MUSICAL_EVENT] });

      const { result } = renderHook(() => useHomeViewModel(navigation));
      await waitFor(() => expect(result.current.savedEventIds).toEqual(['1']));

      expect(result.current.isEventFavorite('1')).toBe(true);
      expect(result.current.isEventFavorite('2')).toBe(false);
    });

    test('handleFavoritePress toggles the wishlist and updates savedEventIds', async () => {
      const navigation = createNavigation();
      getWishlistData.mockResolvedValue({ events: [] });
      toggleWishlistEvent.mockResolvedValue([MUSICAL_EVENT]);

      const { result } = renderHook(() => useHomeViewModel(navigation));
      await waitFor(() => expect(result.current.savedEventIds).toEqual([]));

      let outcome;
      await act(async () => {
        outcome = await result.current.handleFavoritePress(MUSICAL_EVENT);
      });

      expect(outcome).toEqual({ added: true, eventTitle: 'Mamma Mia!' });
      expect(result.current.savedEventIds).toEqual(['1']);
    });

    test('handleFavoritePress reports added: false when removing an already saved event', async () => {
      const navigation = createNavigation();
      getWishlistData.mockResolvedValue({ events: [MUSICAL_EVENT] });
      toggleWishlistEvent.mockResolvedValue([]);

      const { result } = renderHook(() => useHomeViewModel(navigation));
      await waitFor(() => expect(result.current.savedEventIds).toEqual(['1']));

      let outcome;
      await act(async () => {
        outcome = await result.current.handleFavoritePress(MUSICAL_EVENT);
      });

      expect(outcome.added).toBe(false);
      expect(result.current.savedEventIds).toEqual([]);
    });
  });
});