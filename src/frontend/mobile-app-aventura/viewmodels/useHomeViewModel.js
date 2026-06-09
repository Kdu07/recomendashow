import { useCallback, useMemo, useState } from 'react';
import { useFocusEffect } from '@react-navigation/native';

import { getHomeData } from '../repositories/homeRepository';
import {
  getWishlistData,
  getWishlistEventKey,
  toggleWishlistEvent,
} from '../repositories/wishlistRepository';

export default function useHomeViewModel(navigation) {
  const [selectedCategory, setSelectedCategory] = useState('all');
  const [savedEventIds, setSavedEventIds] = useState([]);

  const [hasInterests] = useState(true);
  const [hasLocation] = useState(true);

  const homeData = useMemo(() => getHomeData() || {}, []);

  const categories = homeData.categories || [];
  const heroBanners = homeData.heroBanners || [];
  const theaters = homeData.theaters || [];

  async function loadSavedEvents() {
    const wishlistData = await getWishlistData();
    const nextSavedEventIds = (wishlistData.events || []).map(getWishlistEventKey);

    setSavedEventIds(nextSavedEventIds);
  }

  useFocusEffect(
    useCallback(() => {
      loadSavedEvents();
    }, [])
  );

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
    if (event.showId) {
      navigation.navigate('ShowInfo', { showId: event.showId });
    }
  }

  function handleBannerPress(banner) {
    if (banner.showId) {
      navigation.navigate('ShowInfo', { showId: banner.showId });
    }
  }

  async function handleFavoritePress(event) {
    const eventKey = getWishlistEventKey(event);
    const wasSaved = savedEventIds.includes(eventKey);
    const updatedWishlist = await toggleWishlistEvent(event);
    setSavedEventIds(updatedWishlist.map(getWishlistEventKey));

    return {
      added: !wasSaved,
      eventTitle: event.title,
    };
  }

  function isEventFavorite(event) {
    return savedEventIds.includes(getWishlistEventKey(event));
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
    savedEventIds,

    handleCategoryPress,
    handleEventPress,
    handleBannerPress,
    handleFavoritePress,
    isEventFavorite,
    handleTheaterPress,
    handleEditInterests,
    handleAddLocation,
    handleTabPress,
  };
}
