import { useCallback, useEffect, useMemo, useState } from 'react';
import { useFocusEffect } from '@react-navigation/native';

import {
  getWishlistData,
  removeWishlistEvent,
} from '../repositories/wishlistRepository';

export default function useWishlistViewModel(navigation) {
  const [searchText, setSearchText] = useState('');
  const [savedEvents, setSavedEvents] = useState([]);
  const [isLoading, setIsLoading] = useState(true);

  async function loadWishlist() {
    try {
      setIsLoading(true);

      const wishlistData = await getWishlistData();

      setSavedEvents(wishlistData.events || []);
    } catch (error) {
      console.log('Erro ao carregar wishlist:', error);
    } finally {
      setIsLoading(false);
    }
  }

  useEffect(() => {
    loadWishlist();
  }, []);

  useFocusEffect(
    useCallback(() => {
      loadWishlist();
    }, [])
  );

  function handleSearchChange(value) {
    setSearchText(value);
  }

  function handleEventPress(event) {
    if (event.showId) {
      navigation.navigate('ShowInfo', { showId: event.showId });
    }
  }

  async function handleRemoveFromWishlist(eventId) {
    const removedEvent = savedEvents.find((event) => event.id === eventId);
    const updatedWishlist = await removeWishlistEvent(eventId);
    setSavedEvents(updatedWishlist);

    return {
      eventTitle: removedEvent?.title,
    };
  }

  function handleExplorePress() {
    console.log('Ação futura: navegar para explorar espetáculos');
  }

  const filteredWishlistEvents = useMemo(() => {
    const normalizedSearch = searchText.trim().toLowerCase();

    if (!normalizedSearch) {
      return savedEvents;
    }

    return savedEvents.filter((event) => {
      const title = event.title?.toLowerCase() || '';
      const location = event.location?.toLowerCase() || '';
      const city = event.city?.toLowerCase() || '';

      return (
        title.includes(normalizedSearch) ||
        location.includes(normalizedSearch) ||
        city.includes(normalizedSearch)
      );
    });
  }, [savedEvents, searchText]);

  const hasSavedEvents = savedEvents.length > 0;
  const hasFilteredResults = filteredWishlistEvents.length > 0;

  return {
    searchText,
    savedEvents,
    filteredWishlistEvents,
    hasSavedEvents,
    hasFilteredResults,
    isLoading,

    handleSearchChange,
    handleEventPress,
    handleRemoveFromWishlist,
    handleExplorePress,
    reloadWishlist: loadWishlist,
  };
}
