import {
  trendingEvents,
  recommendedEvents,
  newEvents,
  nearbyEvents,
  categories,
} from './mockHomeData';

const allEvents = [
  ...trendingEvents,
  ...recommendedEvents,
  ...newEvents,
  ...nearbyEvents,
];

export const searchEvents = allEvents.filter(
  (event, index, self) =>
    index === self.findIndex((currentEvent) => currentEvent.title === event.title)
);

export const searchRecommendedEvents = [
  ...recommendedEvents,
  ...trendingEvents,
].filter(
  (event, index, self) =>
    index === self.findIndex((currentEvent) => currentEvent.title === event.title)
).slice(0, 4);

export const searchCategories = categories;