import {
  heroBanners as defaultHeroBanners,
} from '../data/mockHomeData';
import { fetchShows, mapDtoToHomeEvent } from '../services/showService';
import { fetchCategories } from '../services/categoryService';
import { fetchTheaters, mapDtoToTheater } from '../services/theaterService';
import { fetchRecommendations } from '../services/recommendationService';

export async function getHomeData(userId, userCity) {
  try {
    const [showsDto, categoriesDto, theatersDto, recsDto] = await Promise.all([
      fetchShows().catch(() => []),
      fetchCategories().catch(() => [{ id: 'all', label: 'Todos' }]),
      fetchTheaters().catch(() => []),
      userId ? fetchRecommendations(userId).catch(() => []) : Promise.resolve([])
    ]);

    const events = (Array.isArray(showsDto) ? showsDto : []).map(mapDtoToHomeEvent);
    const theaters = (Array.isArray(theatersDto) ? theatersDto : []).map(mapDtoToTheater);

    const heroBanners = events.slice(0, 3).map((event) => ({
      id: `banner-${event.id}`,
      title: event.title,
      showId: event.showId,
      image: event.image,
    }));

    // If API didn't return recommendations, fallback to taking some events
    const recommendedEvents = recsDto.length ? recsDto : events.slice(0, 4);

    // Nearby events based on userCity or just fallback
    const nearbyEvents = userCity 
      ? events.filter(e => e.city?.toLowerCase() === userCity.toLowerCase() || e.location?.toLowerCase().includes(userCity.toLowerCase())).slice(0, 4)
      : events.slice(0, 4);

    // Filter nearbyEvents if we couldn't find any for userCity
    const finalNearbyEvents = nearbyEvents.length ? nearbyEvents : events.slice(0, 4);

    return {
      heroBanners: heroBanners.length ? heroBanners : defaultHeroBanners,
      categories: categoriesDto,
      trendingEvents: events, // Could be sorted by popularity if we had that metric
      recommendedEvents,
      newEvents: [...events].reverse(), // Reversed to simulate "newest" if they are ordered by ID/Date
      nearbyEvents: finalNearbyEvents,
      theaters,
    };
  } catch (error) {
    console.error('getHomeData error:', error);

    return {
      heroBanners: defaultHeroBanners,
      categories: [{ id: 'all', label: 'Todos' }],
      trendingEvents: [],
      recommendedEvents: [],
      newEvents: [],
      nearbyEvents: [],
      theaters: [],
    };
  }
}
