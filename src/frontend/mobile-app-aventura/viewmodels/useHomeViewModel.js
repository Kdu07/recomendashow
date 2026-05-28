import { useMemo, useState } from 'react';
import { getHomeData } from '../repositories/homeRepository';

export default function useHomeViewModel() {
  const [selectedCategory, setSelectedCategory] = useState('all');

  
  const [hasInterests] = useState(true);
  const [hasLocation] = useState(true);

  const homeData = useMemo(() => getHomeData() || {}, []);

  const categories = homeData.categories || [];
  const heroBanners = homeData.heroBanners || [];
  const theaters = homeData.theaters || [];

  function handleCategoryPress(categoryId) {
    setSelectedCategory(categoryId);
  }

  function filterEventsByCategory(events = []) {
    if (selectedCategory === 'all') {
      return events;
    }

    return events.filter((event) => event.categoryId === selectedCategory);
  }

  const trendingEvents = useMemo(
    () => filterEventsByCategory(homeData.trendingEvents || []),
    [homeData.trendingEvents, selectedCategory]
  );

  const recommendedEvents = useMemo(
    () => filterEventsByCategory(homeData.recommendedEvents || []),
    [homeData.recommendedEvents, selectedCategory]
  );

  const newEvents = useMemo(
    () => filterEventsByCategory(homeData.newEvents || []),
    [homeData.newEvents, selectedCategory]
  );

  const nearbyEvents = useMemo(
    () => filterEventsByCategory(homeData.nearbyEvents || []),
    [homeData.nearbyEvents, selectedCategory]
  );

  function handleEventPress(event) {
    console.log('Abrir detalhes do espetáculo:', event.title);
  }

  function handleTheaterPress(theater) {
    console.log('Abrir detalhes do teatro:', theater.name);
  }

  function handleEditInterests() {
    console.log('Ação futura: navegar para edição de interesses');
  }

  function handleAddLocation() {
    console.log('Ação futura: navegar para adicionar localidade');
  }

  function handleTabPress(tab) {
    console.log('Ação futura: navegar para aba:', tab);
  }

  return {
    selectedCategory,
    hasInterests,
    hasLocation,

    heroBanners,
    categories,
    theaters,

    trendingEvents,
    recommendedEvents,
    newEvents,
    nearbyEvents,

    handleCategoryPress,
    handleEventPress,
    handleTheaterPress,
    handleEditInterests,
    handleAddLocation,
    handleTabPress,
  };
}