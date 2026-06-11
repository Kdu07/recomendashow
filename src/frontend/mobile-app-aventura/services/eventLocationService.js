import { fetchTheaters, fetchTheaterShows, mapDtoToTheater } from './theaterService';

let cachedContext = null;

function read(dto, pascal, camel) {
  return dto?.[pascal] ?? dto?.[camel];
}

function normalize(value) {
  return String(value ?? '').trim().toLowerCase();
}

function uniqueByNormalized(values) {
  const seen = new Set();

  return values.filter((value) => {
    const key = normalize(value);
    if (!key || seen.has(key)) {
      return false;
    }

    seen.add(key);
    return true;
  });
}

export async function fetchEventLocationContext() {
  if (cachedContext) {
    return cachedContext;
  }

  const theatersDto = await fetchTheaters();
  const theaters = (Array.isArray(theatersDto) ? theatersDto : []).map(mapDtoToTheater);
  const eventLocations = {};

  await Promise.all(
    theaters.map(async (theater) => {
      try {
        const shows = await fetchTheaterShows(theater.id);

        (Array.isArray(shows) ? shows : []).forEach((show) => {
          const showId = read(show, 'Id', 'id');
          if (!showId || eventLocations[showId]) {
            return;
          }

          eventLocations[showId] = {
            city: theater.city || '',
            venueName: theater.name || '',
            address: theater.location || '',
          };
        });
      } catch (error) {
        console.warn('Nao foi possivel relacionar teatro e espetaculos:', error);
      }
    })
  );

  cachedContext = {
    theaters,
    eventLocations,
    locations: uniqueByNormalized(theaters.map((theater) => theater.city)).sort((a, b) =>
      a.localeCompare(b, 'pt-BR')
    ),
  };

  return cachedContext;
}

export function matchesLocation(event, selectedLocation) {
  if (!selectedLocation || selectedLocation === 'all') {
    return true;
  }

  return normalize(event.city) === normalize(selectedLocation);
}
