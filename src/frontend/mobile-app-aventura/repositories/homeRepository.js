import { fetchShows, mapDtoToHomeEvent } from '../services/showService';
import { fetchCategories } from '../services/categoryService';
import { fetchRecommendations } from '../services/recommendationService';
import { fetchEventLocationContext, matchesLocation } from '../services/eventLocationService';

export async function getHomeData(userId, userCity) {
  try {
    const [showsDto, categoriesDto, locationContext, recsDto] = await Promise.all([
      fetchShows().catch(() => []),
      fetchCategories().catch(() => [{ id: 'all', label: 'Todos' }]),
      fetchEventLocationContext().catch(() => ({ theaters: [], eventLocations: {}, locations: [] })),
      userId ? fetchRecommendations(userId).catch(() => []) : Promise.resolve([])
    ]);

    const events = (Array.isArray(showsDto) ? showsDto : []).map((show) =>
      mapDtoToHomeEvent(show, locationContext.eventLocations[show.Id ?? show.id])
    );
    const theaters = locationContext.theaters || [];

    const heroBanners = events.slice(0, 3).map((event) => ({
      id: `banner-${event.id}`,
      title: event.title,
      showId: event.showId,
      image: event.image,
    }));

    // If API didn't return recommendations, fallback to taking some events
    const recommendedEvents = recsDto.length
      ? recsDto.map((event) => ({
          ...event,
          ...(locationContext.eventLocations[event.showId] || {}),
          location: locationContext.eventLocations[event.showId]?.venueName || event.location,
        }))
      : events.slice(0, 4);

    // Nearby events based on the city saved during registration/onboarding/profile.
    const nearbyEvents = userCity
      ? events.filter((event) => matchesLocation(event, userCity)).slice(0, 4)
      : events.slice(0, 4);

    // Filter nearbyEvents if we couldn't find any for userCity
    const finalNearbyEvents = nearbyEvents.length ? nearbyEvents : events.slice(0, 4);

    return {
      heroBanners,
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
      heroBanners: [],
      categories: [{ id: 'all', label: 'Todos' }],
      trendingEvents: [],
      recommendedEvents: [],
      newEvents: [],
      nearbyEvents: [],
      theaters: [],
    };
  }
}
