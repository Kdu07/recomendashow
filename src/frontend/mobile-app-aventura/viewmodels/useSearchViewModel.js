import { useCallback, useMemo, useState } from 'react';
import { useFocusEffect } from '@react-navigation/native';

import { getSearchData } from '../repositories/searchRepository';
import {
  getWishlistData,
  toggleWishlistEvent,
} from '../repositories/wishlistRepository';

export default function useSearchViewModel() {
  const [searchText, setSearchText] = useState('');
  const [selectedCategory, setSelectedCategory] = useState('all');
  const [savedEventIds, setSavedEventIds] = useState([]);

  const searchData = useMemo(() => getSearchData() || {}, []);

  const events = searchData.events || [];
  const categories = searchData.categories || [];
  const recommendedEvents = searchData.recommendedEvents || [];

  const isSearching =
    searchText.trim().length > 0 || selectedCategory !== 'all';

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

  function handleClearSearch() {
    setSearchText('');
    setSelectedCategory('all');
  }

  function handleEventPress(event) {
    console.log('Abrir detalhes do espetaculo:', event.title);
  }

  async function handleFavoritePress(event) {
    const updatedWishlist = await toggleWishlistEvent(event);
    setSavedEventIds(updatedWishlist.map((savedEvent) => savedEvent.id));
  }

  function isEventFavorite(eventId) {
    return savedEventIds.includes(eventId);
  }

  function handleTabPress(tab) {
    console.log('Acao futura: navegar para aba:', tab);
  }

  const filteredEvents = useMemo(() => {
    const normalizedSearch = searchText.trim().toLowerCase();

    if (!isSearching) {
      return recommendedEvents;
    }

    return events.filter((event) => {
      const title = event.title?.toLowerCase() || '';
      const location = event.location?.toLowerCase() || '';
      const city = event.city?.toLowerCase() || '';
      const category = event.categoryId?.toLowerCase() || '';

      const matchesText =
        !normalizedSearch ||
        title.includes(normalizedSearch) ||
        location.includes(normalizedSearch) ||
        city.includes(normalizedSearch) ||
        category.includes(normalizedSearch);

      const matchesCategory =
        selectedCategory === 'all' || event.categoryId === selectedCategory;

      return matchesText && matchesCategory;
    });
  }, [events, recommendedEvents, searchText, selectedCategory, isSearching]);

  const hasResults = filteredEvents.length > 0;

  const sectionTitle = isSearching
    ? 'Resultados da busca'
    : 'Recomendado para voce';

  const sectionSubtitle = isSearching
    ? hasResults
      ? `${filteredEvents.length} espetaculos encontrados`
      : 'Nenhum espetaculo encontrado'
    : 'Algumas sugestoes para comecar sua busca';

  return {
    searchText,
    selectedCategory,
    categories,
    filteredEvents,
    hasResults,
    isSearching,
    sectionTitle,
    sectionSubtitle,
    savedEventIds,

    handleSearchChange,
    handleCategoryPress,
    handleClearSearch,
    handleEventPress,
    handleFavoritePress,
    isEventFavorite,
    handleTabPress,
  };
}
