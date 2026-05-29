import { useState } from 'react';
import { Linking } from 'react-native';
import { MOCK_SHOWS } from '../models/showInfoData';

export function useShowInfoViewModel(showId = '1') {
  if (!MOCK_SHOWS[showId]) {
    return { show: null, error: true, subtitle: null, toggleFavorite: () => {}, onPressTickets: () => {} };
  }

  const [show, setShow] = useState(MOCK_SHOWS[showId]);

  const subtitle = show.date.status;

  function toggleFavorite() {
    setShow((prev) => ({ ...prev, isFavorited: !prev.isFavorited }));
  }

  function onPressTickets() {
    if (show?.ticket?.url) {
      Linking.openURL(show.ticket.url).catch(console.error);
    }
  }

  return {
    show,
    subtitle,
    toggleFavorite,
    onPressTickets,
  };
}
