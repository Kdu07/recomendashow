jest.mock('../services/theaterService', () => ({
  fetchTheaters: jest.fn(),
  fetchTheaterShows: jest.fn(),
  mapDtoToTheater: jest.fn(),
}));

describe('eventLocationService', () => {
  beforeEach(() => {
    jest.resetModules();
    jest.clearAllMocks();
  });

  function loadModules() {
    const eventLocationService = require('../services/eventLocationService');
    const theaterService = require('../services/theaterService');
    return { eventLocationService, theaterService };
  }

  describe('fetchEventLocationContext', () => {
    test('fetches theaters and builds the eventLocations map from each theater shows', async () => {
      const { eventLocationService, theaterService } = loadModules();

      theaterService.fetchTheaters.mockResolvedValue([{ id: 't1' }]);
      theaterService.mapDtoToTheater.mockReturnValue({
        id: 't1',
        name: 'Teatro Riachuelo',
        city: 'Rio de Janeiro',
        location: 'Rua do Passeio, 40',
      });
      theaterService.fetchTheaterShows.mockResolvedValue([{ Id: 'show-1' }]);

      const context = await eventLocationService.fetchEventLocationContext();

      expect(context.eventLocations['show-1']).toEqual({
        city: 'Rio de Janeiro',
        venueName: 'Teatro Riachuelo',
        address: 'Rua do Passeio, 40',
      });
    });

    test('caches the result so a second call does not hit the API again', async () => {
      const { eventLocationService, theaterService } = loadModules();

      theaterService.fetchTheaters.mockResolvedValue([]);

      const first = await eventLocationService.fetchEventLocationContext();
      const second = await eventLocationService.fetchEventLocationContext();

      expect(first).toBe(second);
      expect(theaterService.fetchTheaters).toHaveBeenCalledTimes(1);
    });

    test('treats a non-array theaters response as an empty list', async () => {
      const { eventLocationService, theaterService } = loadModules();

      theaterService.fetchTheaters.mockResolvedValue(null);

      const context = await eventLocationService.fetchEventLocationContext();

      expect(context.theaters).toEqual([]);
      expect(context.locations).toEqual([]);
    });

    test('keeps going when fetchTheaterShows fails for one theater', async () => {
      const { eventLocationService, theaterService } = loadModules();
      jest.spyOn(console, 'warn').mockImplementation(() => {});

      theaterService.fetchTheaters.mockResolvedValue([{ id: 't1' }]);
      theaterService.mapDtoToTheater.mockReturnValue({
        id: 't1',
        name: 'Teatro X',
        city: 'São Paulo',
        location: 'Rua A',
      });
      theaterService.fetchTheaterShows.mockRejectedValue(new Error('network error'));

      const context = await eventLocationService.fetchEventLocationContext();

      expect(context.eventLocations).toEqual({});
      expect(console.warn).toHaveBeenCalled();

      console.warn.mockRestore();
    });

    test('treats a non-array shows response as an empty list', async () => {
      const { eventLocationService, theaterService } = loadModules();

      theaterService.fetchTheaters.mockResolvedValue([{ id: 't1' }]);
      theaterService.mapDtoToTheater.mockReturnValue({ id: 't1', name: 'Teatro X', city: 'São Paulo' });
      theaterService.fetchTheaterShows.mockResolvedValue(null);

      const context = await eventLocationService.fetchEventLocationContext();

      expect(context.eventLocations).toEqual({});
    });

    test('falls back to empty strings when the theater has no city, name or location', async () => {
      const { eventLocationService, theaterService } = loadModules();

      theaterService.fetchTheaters.mockResolvedValue([{ id: 't1' }]);
      theaterService.mapDtoToTheater.mockReturnValue({ id: 't1' });
      theaterService.fetchTheaterShows.mockResolvedValue([{ id: 'show-1' }]);

      const context = await eventLocationService.fetchEventLocationContext();

      expect(context.eventLocations['show-1']).toEqual({
        city: '',
        venueName: '',
        address: '',
      });
    });

    test('ignores shows without an id and does not overwrite an already mapped show', async () => {
      const { eventLocationService, theaterService } = loadModules();

      theaterService.fetchTheaters.mockResolvedValue([{ id: 't1' }, { id: 't2' }]);
      theaterService.mapDtoToTheater
        .mockReturnValueOnce({ id: 't1', name: 'Teatro A', city: 'São Paulo' })
        .mockReturnValueOnce({ id: 't2', name: 'Teatro B', city: 'Rio de Janeiro' });
      theaterService.fetchTheaterShows
        .mockResolvedValueOnce([{ Id: null }, { id: 'show-1' }])
        .mockResolvedValueOnce([{ id: 'show-1' }]);

      const context = await eventLocationService.fetchEventLocationContext();

      expect(context.eventLocations['show-1'].city).toBe('São Paulo');
      expect(Object.keys(context.eventLocations)).toEqual(['show-1']);
    });

    test('returns the unique theater cities sorted alphabetically (pt-BR)', async () => {
      const { eventLocationService, theaterService } = loadModules();

      theaterService.fetchTheaters.mockResolvedValue([{ id: 't1' }, { id: 't2' }, { id: 't3' }]);
      theaterService.mapDtoToTheater
        .mockReturnValueOnce({ id: 't1', name: 'Teatro A', city: 'São Paulo' })
        .mockReturnValueOnce({ id: 't2', name: 'Teatro B', city: 'Rio de Janeiro' })
        .mockReturnValueOnce({ id: 't3', name: 'Teatro C', city: '  são paulo  ' });
      theaterService.fetchTheaterShows.mockResolvedValue([]);

      const context = await eventLocationService.fetchEventLocationContext();

      expect(context.locations).toEqual(['Rio de Janeiro', 'São Paulo']);
    });
  });

  describe('matchesLocation', () => {
    test('returns true when selectedLocation is falsy or "all"', () => {
      const { eventLocationService } = loadModules();

      expect(eventLocationService.matchesLocation({ city: 'São Paulo' }, undefined)).toBe(true);
      expect(eventLocationService.matchesLocation({ city: 'São Paulo' }, '')).toBe(true);
      expect(eventLocationService.matchesLocation({ city: 'São Paulo' }, 'all')).toBe(true);
    });

    test('compares the event city and the selected location case-insensitively, trimmed', () => {
      const { eventLocationService } = loadModules();

      expect(eventLocationService.matchesLocation({ city: 'São Paulo' }, '  SÃO PAULO  ')).toBe(true);
      expect(eventLocationService.matchesLocation({ city: 'São Paulo' }, 'Rio de Janeiro')).toBe(false);
    });

    test('treats a missing event city as not matching a specific location', () => {
      const { eventLocationService } = loadModules();

      expect(eventLocationService.matchesLocation({}, 'São Paulo')).toBe(false);
    });
  });
});