import { useEffect, useState } from 'react';
import { Linking } from 'react-native';
import { toggleWishlistEvent } from '../repositories/wishlistRepository';
import { fetchShowById, mapDtoToShowInfo } from '../services/showService';

export function useShowInfoViewModel(showId = '1') {
  const [show, setShow] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(false);

  useEffect(() => {
    let isMounted = true;

    async function loadShow() {
      try {
        const dto = await fetchShowById(showId);
        if (isMounted) {
          setShow(mapDtoToShowInfo(dto));
        }
      } catch (loadError) {
        console.error('Failed to load show details:', loadError);
        if (isMounted) {
          setError(true);
        }
      } finally {
        if (isMounted) {
          setLoading(false);
        }
      }
    }

    loadShow();

    return () => {
      isMounted = false;
    };
  }, [showId]);

  const subtitle = show?.date?.status ?? '';

  async function toggleFavorite() {
    if (!show) {
      return { added: false, eventTitle: '' };
    }

    const wasFavorited = show.isFavorited;
    const updatedShow = {
      ...show,
      isFavorited: !wasFavorited,
    };

    setShow(updatedShow);

    await toggleWishlistEvent({
      id: show.id,
      showId: show.id,
      title: show.title,
      location: show.location?.venue,
      city: show.location?.city,
      categoryId: 'musical',
      image: show.heroImage,
    });

    return {
      added: !wasFavorited,
      eventTitle: show.title,
    };
  }

  function onPressTickets() {
    if (show?.ticket?.url) {
      Linking.openURL(show.ticket.url).catch(console.error);
    }
  }

  function onPressVenue() {
    if (show?.location?.venueUrl) {
      Linking.openURL(show.location.venueUrl).catch(console.error);
    }
  }

  return {
    show,
    loading,
    error,
    subtitle,
    toggleFavorite,
    onPressTickets,
    onPressVenue,
  };
}
