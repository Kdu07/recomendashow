import { useCallback, useEffect, useState } from 'react';
import { useFocusEffect } from '@react-navigation/native';

import { getSearchData, searchEventsData } from '../repositories/searchRepository';
import {
  getWishlistData,
  toggleWishlistEvent,
} from '../repositories/wishlistRepository';
import { getAuthSession } from '../services/tokenStorageService';

export default function useSearchViewModel(navigation) {
  const [searchText, setSearchText] = useState('');
  const [selectedCategory, setSelectedCategory] = useState('all');
  const [selectedLocation, setSelectedLocation] = useState('all');
  const [savedEventIds, setSavedEventIds] = useState([]);

  const [categories, setCategories] = useState([{ id: 'all', label: 'Todos' }]);
  const [locations, setLocations] = useState([{ id: 'all', label: 'Todas' }]);
  const [recommendedEvents, setRecommendedEvents] = useState([]);
  const [filteredEvents, setFilteredEvents] = useState([]);

  const isSearching =
    searchText.trim().length > 0 || selectedCategory !== 'all' || selectedLocation !== 'all';

  // Load initial search data (categories and recommendations)
  useEffect(() => {
    let isMounted = true;
    async function loadInitialData() {
      try {
        const session = await getAuthSession();
        const data = await getSearchData(session?.usuarioId);
        if (isMounted) {
          setCategories(data.categories || []);
          setLocations(data.locations || [{ id: 'all', label: 'Todas' }]);
          setRecommendedEvents(data.recommendedEvents || []);
        }
      } catch (error) {
        console.error('Failed to load initial search data', error);
      }
    }
    loadInitialData();
    return () => { isMounted = false; };
  }, []);

  // Perform search when searchText or selectedCategory changes
  useEffect(() => {
    let isMounted = true;
    const timeoutId = setTimeout(async () => {
      if (!isSearching) {
        if (isMounted) setFilteredEvents([]);
        return;
      }
      try {
        const results = await searchEventsData(
          searchText.trim(),
          selectedCategory === 'all' ? null : selectedCategory,
          selectedLocation === 'all' ? null : selectedLocation
        );
        if (isMounted) {
          setFilteredEvents(results || []);
        }
      } catch (error) {
        console.error('Failed to search events', error);
      }
    }, 300); // 300ms debounce

    return () => {
      isMounted = false;
      clearTimeout(timeoutId);
    };
  }, [searchText, selectedCategory, selectedLocation, isSearching]);

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

  function handleSearchChange(value) {
    setSearchText(value);
  }

  function handleCategoryPress(categoryId) {
    setSelectedCategory(categoryId);
  }

  function handleLocationPress(locationId) {
    setSelectedLocation(locationId);
  }

  function handleClearSearch() {
    setSearchText('');
    setSelectedCategory('all');
    setSelectedLocation('all');
  }

  function handleEventPress(event) {
    if (event.showId) {
      navigation.navigate('ShowInfo', { showId: event.showId });
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

  function handleTabPress(tab) {
    console.log('Ação futura: navegar para aba:', tab);
  }

  const currentEvents = isSearching ? filteredEvents : recommendedEvents;
  const hasResults = currentEvents.length > 0;

  const sectionTitle = isSearching
    ? 'Resultados da busca'
    : 'Recomendado para você';

  const sectionSubtitle = isSearching
    ? hasResults
      ? `${currentEvents.length} espetáculos encontrados`
      : 'Nenhum espetáculo encontrado'
    : 'Algumas sugestões para começar sua busca';

  return {
    searchText,
    selectedCategory,
    selectedLocation,
    categories,
    locations,
    filteredEvents: currentEvents,
    hasResults,
    isSearching,
    sectionTitle,
    sectionSubtitle,
    savedEventIds,

    handleSearchChange,
    handleCategoryPress,
    handleLocationPress,
    handleClearSearch,
    handleEventPress,
    handleFavoritePress,
    isEventFavorite,
    handleTabPress,
  };
}
