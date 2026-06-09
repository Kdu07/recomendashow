import {
  heroBanners as defaultHeroBanners,
  categories,
  theaters,
} from '../data/mockHomeData';
import { fetchShows, mapDtoToHomeEvent } from '../services/showService';

export async function getHomeData() {
  try {
    const shows = await fetchShows();
    const events = (Array.isArray(shows) ? shows : []).map(mapDtoToHomeEvent);

    const heroBanners = events.slice(0, 3).map((event) => ({
      id: `banner-${event.id}`,
      title: event.title,
      showId: event.showId,
      image: event.image,
    }));

    return {
      heroBanners: heroBanners.length ? heroBanners : defaultHeroBanners,
      categories,
      trendingEvents: events,
      recommendedEvents: events.slice(0, 4),
      newEvents: events,
      nearbyEvents: events.slice(0, 4),
      theaters,
    };
  } catch (error) {
    console.error('getHomeData error:', error);

    return {
      heroBanners: defaultHeroBanners,
      categories,
      trendingEvents: [],
      recommendedEvents: [],
      newEvents: [],
      nearbyEvents: [],
      theaters,
    };
  }
}
