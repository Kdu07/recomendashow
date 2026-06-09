import { useCallback, useEffect, useMemo, useState } from 'react';
import { useFocusEffect } from '@react-navigation/native';

import { categories as defaultCategories, theaters as defaultTheaters } from '../data/mockHomeData';
import { getHomeData } from '../repositories/homeRepository';
import {
  getWishlistData,
  toggleWishlistEvent,
} from '../repositories/wishlistRepository';
import { getAuthSession } from '../services/tokenStorageService';
import { fetchPreferences } from '../services/preferenceService';

export default function useHomeViewModel(navigation) {
  const [selectedCategory, setSelectedCategory] = useState('all');
  const [savedEventIds, setSavedEventIds] = useState([]);
  const [homeData, setHomeData] = useState({
    categories: defaultCategories,
    heroBanners: [],
    theaters: defaultTheaters,
    trendingEvents: [],
    recommendedEvents: [],
    newEvents: [],
    nearbyEvents: [],
  });

  const [hasInterests, setHasInterests] = useState(false);
  const [hasLocation, setHasLocation] = useState(false);

  useEffect(() => {
    let isMounted = true;

    async function loadHomeData() {
      try {
        const session = await getAuthSession();
        const userId = session?.usuarioId;
        
        let userCity = null;
        if (userId) {
          const prefs = await fetchPreferences(userId);
          if (prefs) {
            setHasInterests(prefs.generos?.length > 0 || prefs.tags?.length > 0);
            setHasLocation(!!prefs.cidade);
            userCity = prefs.cidade;
          }
        }

        const data = await getHomeData(userId, userCity);
        if (isMounted) {
          setHomeData(data);
        }
      } catch (error) {
        console.error('Failed to load home data:', error);
      }
    }

    loadHomeData();

    return () => {
      isMounted = false;
    };
  }, []);

  const categories = homeData.categories || [];
  const heroBanners = homeData.heroBanners || [];
  const theaters = homeData.theaters || [];

  async function loadSavedEvents() {
    const wishlistData = await getWishlistData();
    const nextSavedEventIds = (wishlistData.events || []).map((event) => event.id);

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
    const wasSaved = savedEventIds.includes(event.id);
    const updatedWishlist = await toggleWishlistEvent(event);
    setSavedEventIds(updatedWishlist.map((savedEvent) => savedEvent.id));

    return {
      added: !wasSaved,
      eventTitle: event.title,
    };
  }

  function isEventFavorite(eventId) {
    return savedEventIds.includes(eventId);
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
