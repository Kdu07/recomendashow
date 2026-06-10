import { useState } from 'react';
import { Linking } from 'react-native';
import { MOCK_SHOWS } from '../models/showInfoData';
import { toggleWishlistEvent } from '../repositories/wishlistRepository';

export function useShowInfoViewModel(showId = '1') {
  if (!MOCK_SHOWS[showId]) {
    return { show: null, error: true, subtitle: null, toggleFavorite: () => {}, onPressTickets: () => {} };
  }

  const [show, setShow] = useState(MOCK_SHOWS[showId]);

  const subtitle = show.date.status;

  async function toggleFavorite() {
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
    subtitle,
    toggleFavorite,
    onPressTickets,
    onPressVenue,
  };
}
