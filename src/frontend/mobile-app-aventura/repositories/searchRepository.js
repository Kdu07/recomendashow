import {
  searchEvents,
  searchCategories,
  searchRecommendedEvents,
} from '../data/mockSearchData';

export function getSearchData() {
  return {
    events: searchEvents,
    categories: searchCategories,
    recommendedEvents: searchRecommendedEvents,
  };
}