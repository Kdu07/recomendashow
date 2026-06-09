import { searchShows, mapDtoToHomeEvent } from '../services/showService';
import { fetchCategories } from '../services/categoryService';
import { fetchRecommendations } from '../services/recommendationService';

export async function getSearchData(userId) {
  try {
    const [categoriesDto, recsDto] = await Promise.all([
      fetchCategories().catch(() => [{ id: 'all', label: 'Todos' }]),
      userId ? fetchRecommendations(userId).catch(() => []) : Promise.resolve([])
    ]);

    return {
      categories: categoriesDto,
      recommendedEvents: recsDto,
    };
  } catch (error) {
    console.error('getSearchData error:', error);
    return {
      categories: [{ id: 'all', label: 'Todos' }],
      recommendedEvents: [],
    };
  }
}

export async function searchEventsData(query, category) {
  try {
    const showsDto = await searchShows(query, category);
    return (Array.isArray(showsDto) ? showsDto : []).map(mapDtoToHomeEvent);
  } catch (error) {
    console.error('searchEventsData error:', error);
    return [];
  }
}