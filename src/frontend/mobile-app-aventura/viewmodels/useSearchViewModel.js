import { useMemo, useState } from 'react';

import { getSearchData } from '../repositories/searchRepository';

export default function useSearchViewModel() {
  const [searchText, setSearchText] = useState('');
  const [selectedCategory, setSelectedCategory] = useState('all');

  const searchData = useMemo(() => getSearchData() || {}, []);

  const events = searchData.events || [];
  const categories = searchData.categories || [];
  const recommendedEvents = searchData.recommendedEvents || [];

  const isSearching =
    searchText.trim().length > 0 || selectedCategory !== 'all';

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
    console.log('Abrir detalhes do espetáculo:', event.title);
  }

  function handleTabPress(tab) {
    console.log('Ação futura: navegar para aba:', tab);
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
    : 'Recomendado para você';

  const sectionSubtitle = isSearching
    ? hasResults
      ? `${filteredEvents.length} espetáculos encontrados`
      : 'Nenhum espetáculo encontrado'
    : 'Algumas sugestões para começar sua busca';

  return {
    searchText,
    selectedCategory,
    categories,
    filteredEvents,
    hasResults,
    isSearching,
    sectionTitle,
    sectionSubtitle,

    handleSearchChange,
    handleCategoryPress,
    handleClearSearch,
    handleEventPress,
    handleTabPress,
  };
}