import { searchShows, mapDtoToHomeEvent } from '../services/showService';
import { fetchCategories } from '../services/categoryService';
import { fetchRecommendations } from '../services/recommendationService';
import { fetchEventLocationContext, matchesLocation } from '../services/eventLocationService';

export async function getSearchData(userId) {
  try {
    const [categoriesDto, locationContext, recsDto] = await Promise.all([
      fetchCategories().catch(() => [{ id: 'all', label: 'Todos' }]),
      fetchEventLocationContext().catch(() => ({ eventLocations: {}, locations: [] })),
      userId ? fetchRecommendations(userId).catch(() => []) : Promise.resolve([])
    ]);

    return {
      categories: categoriesDto,
      locations: [
        { id: 'all', label: 'Todas' },
        ...(locationContext.locations || []).map((location) => ({ id: location, label: location })),
      ],
      recommendedEvents: recsDto.map((event) => ({
        ...event,
        ...(locationContext.eventLocations[event.showId] || {}),
        location: locationContext.eventLocations[event.showId]?.venueName || event.location,
      })),
    };
  } catch (error) {
    console.error('getSearchData error:', error);
    return {
      categories: [{ id: 'all', label: 'Todos' }],
      locations: [{ id: 'all', label: 'Todas' }],
      recommendedEvents: [],
    };
  }
}

export async function searchEventsData(query, category, location) {
  try {
    const [showsDto, locationContext] = await Promise.all([
      searchShows(query, category),
      fetchEventLocationContext().catch(() => ({ eventLocations: {} })),
    ]);

    return (Array.isArray(showsDto) ? showsDto : [])
      .map((show) => mapDtoToHomeEvent(show, locationContext.eventLocations[show.Id ?? show.id]))
      .filter((event) => matchesLocation(event, location));
  } catch (error) {
    console.error('searchEventsData error:', error);
    return [];
  }
}
