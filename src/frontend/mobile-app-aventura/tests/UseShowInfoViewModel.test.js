import { renderHook, act, waitFor } from '@testing-library/react';
import { Linking } from 'react-native';

import { useShowInfoViewModel } from '../viewmodels/useShowInfoViewModel';
import { checkEventIsSaved, toggleWishlistEvent } from '../repositories/wishlistRepository';
import { fetchShowById, mapDtoToShowInfo } from '../services/showService';

jest.mock('../repositories/wishlistRepository', () => ({
  checkEventIsSaved: jest.fn(),
  toggleWishlistEvent: jest.fn(),
}));

jest.mock('../services/showService', () => ({
  fetchShowById: jest.fn(),
  mapDtoToShowInfo: jest.fn(),
}));

jest.mock('react-native', () => ({
  Linking: {
    openURL: jest.fn(),
  },
}));

const MAPPED_SHOW = {
  id: 'show-1',
  title: 'Mamma Mia!',
  heroImage: { uri: 'http://api.test/img' },
  date: { status: 'Em cartaz' },
  location: { venue: 'Teatro Riachuelo', city: 'Rio de Janeiro', venueUrl: 'https://teatro.com' },
  ticket: { url: 'https://bilheteria.com' },
};

describe('useShowInfoViewModel', () => {
  beforeEach(() => {
    jest.clearAllMocks();
    jest.spyOn(console, 'error').mockImplementation(() => {});
    fetchShowById.mockResolvedValue({ id: 'show-1' });
    mapDtoToShowInfo.mockReturnValue(MAPPED_SHOW);
    checkEventIsSaved.mockResolvedValue(false);
    Linking.openURL.mockResolvedValue(undefined);
  });

  afterEach(() => {
    console.error.mockRestore();
  });

  describe('initial load', () => {
    test('starts in a loading state and stops loading once the show resolves', async () => {
      const { result } = renderHook(() => useShowInfoViewModel('show-1'));

      expect(result.current.loading).toBe(true);

      await waitFor(() => expect(result.current.loading).toBe(false));
    });

    test('fetches the show by id and merges the favorited flag', async () => {
      checkEventIsSaved.mockResolvedValue(true);

      const { result } = renderHook(() => useShowInfoViewModel('show-1'));
      await waitFor(() => expect(result.current.loading).toBe(false));

      expect(fetchShowById).toHaveBeenCalledWith('show-1');
      expect(result.current.show).toEqual({ ...MAPPED_SHOW, isFavorited: true });
    });

    test('defaults showId to "1" when none is provided', async () => {
      renderHook(() => useShowInfoViewModel());

      await waitFor(() => expect(fetchShowById).toHaveBeenCalledWith('1'));
    });

    test('treats a failed favorite check as not favorited', async () => {
      checkEventIsSaved.mockRejectedValue(new Error('network down'));

      const { result } = renderHook(() => useShowInfoViewModel('show-1'));
      await waitFor(() => expect(result.current.loading).toBe(false));

      expect(result.current.show.isFavorited).toBe(false);
    });

    test('sets error to true and keeps show null when fetchShowById rejects', async () => {
      fetchShowById.mockRejectedValue(new Error('not found'));

      const { result } = renderHook(() => useShowInfoViewModel('missing'));
      await waitFor(() => expect(result.current.loading).toBe(false));

      expect(result.current.error).toBe(true);
      expect(result.current.show).toBeNull();
    });

    test('reloads the show when showId changes', async () => {
      const { rerender } = renderHook(({ showId }) => useShowInfoViewModel(showId), {
        initialProps: { showId: 'show-1' },
      });

      await waitFor(() => expect(fetchShowById).toHaveBeenCalledWith('show-1'));

      rerender({ showId: 'show-2' });

      await waitFor(() => expect(fetchShowById).toHaveBeenCalledWith('show-2'));
    });
  });

  describe('subtitle', () => {
    test('exposes the date status from the loaded show', async () => {
      const { result } = renderHook(() => useShowInfoViewModel('show-1'));
      await waitFor(() => expect(result.current.loading).toBe(false));

      expect(result.current.subtitle).toBe('Em cartaz');
    });

    test('is an empty string while the show has not loaded yet', () => {
      const { result } = renderHook(() => useShowInfoViewModel('show-1'));

      expect(result.current.subtitle).toBe('');
    });
  });

  describe('toggleFavorite', () => {
    test('returns added: false without side effects when the show has not loaded', async () => {
      const { result } = renderHook(() => useShowInfoViewModel('show-1'));

      let outcome;
      await act(async () => {
        outcome = await result.current.toggleFavorite();
      });

      expect(outcome).toEqual({ added: false, eventTitle: '' });
      expect(toggleWishlistEvent).not.toHaveBeenCalled();
    });

    test('optimistically flips isFavorited and calls toggleWishlistEvent with the mapped payload', async () => {
      toggleWishlistEvent.mockResolvedValue(undefined);
      const { result } = renderHook(() => useShowInfoViewModel('show-1'));
      await waitFor(() => expect(result.current.loading).toBe(false));

      let outcome;
      await act(async () => {
        outcome = await result.current.toggleFavorite();
      });

      expect(result.current.show.isFavorited).toBe(true);
      expect(outcome).toEqual({ added: true, eventTitle: 'Mamma Mia!' });
      expect(toggleWishlistEvent).toHaveBeenCalledWith({
        id: 'show-1',
        showId: 'show-1',
        title: 'Mamma Mia!',
        location: 'Teatro Riachuelo',
        city: 'Rio de Janeiro',
        categoryId: 'musical',
        image: { uri: 'http://api.test/img' },
      });
    });

    test('reports added: false when the show was already favorited', async () => {
      checkEventIsSaved.mockResolvedValue(true);
      toggleWishlistEvent.mockResolvedValue(undefined);
      const { result } = renderHook(() => useShowInfoViewModel('show-1'));
      await waitFor(() => expect(result.current.loading).toBe(false));

      let outcome;
      await act(async () => {
        outcome = await result.current.toggleFavorite();
      });

      expect(outcome.added).toBe(false);
      expect(result.current.show.isFavorited).toBe(false);
    });
  });

  describe('onPressTickets', () => {
    test('opens the ticket url when present', async () => {
      const { result } = renderHook(() => useShowInfoViewModel('show-1'));
      await waitFor(() => expect(result.current.loading).toBe(false));

      result.current.onPressTickets();

      expect(Linking.openURL).toHaveBeenCalledWith('https://bilheteria.com');
    });

    test('does nothing when there is no ticket url', async () => {
      mapDtoToShowInfo.mockReturnValue({ ...MAPPED_SHOW, ticket: { url: '' } });
      const { result } = renderHook(() => useShowInfoViewModel('show-1'));
      await waitFor(() => expect(result.current.loading).toBe(false));

      result.current.onPressTickets();

      expect(Linking.openURL).not.toHaveBeenCalled();
    });

    test('does nothing while the show has not loaded yet', () => {
      const { result } = renderHook(() => useShowInfoViewModel('show-1'));

      result.current.onPressTickets();

      expect(Linking.openURL).not.toHaveBeenCalled();
    });
  });

  describe('onPressVenue', () => {
    test('opens the venue url when present', async () => {
      const { result } = renderHook(() => useShowInfoViewModel('show-1'));
      await waitFor(() => expect(result.current.loading).toBe(false));

      result.current.onPressVenue();

      expect(Linking.openURL).toHaveBeenCalledWith('https://teatro.com');
    });

    test('does nothing when there is no venue url', async () => {
      mapDtoToShowInfo.mockReturnValue({ ...MAPPED_SHOW, location: { venue: 'Teatro X' } });
      const { result } = renderHook(() => useShowInfoViewModel('show-1'));
      await waitFor(() => expect(result.current.loading).toBe(false));

      result.current.onPressVenue();

      expect(Linking.openURL).not.toHaveBeenCalled();
    });
  });
});