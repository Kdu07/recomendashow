import {
  recommendedEvents,
  trendingEvents,
  newEvents,
} from './mockHomeData';

const savedEvents = [
  recommendedEvents[0],
  trendingEvents[0],
  newEvents[1],
].filter(Boolean);

export const wishlistEvents = savedEvents.filter(
  (event, index, self) =>
    index === self.findIndex((currentEvent) => currentEvent.title === event.title)
);