import { useState } from 'react';
import { Linking } from 'react-native';
import { MOCK_SHOWS } from '../models/showInfoData';

export function useShowInfoViewModel(showId = '1') {
  const [show, setShow] = useState(MOCK_SHOWS[showId]);

  const subtitle = show.date.status;

  function toggleFavorite() {
    setShow((prev) => ({ ...prev, isFavorited: !prev.isFavorited }));
  }

  function onPressTickets() {
    Linking.openURL(show.ticket.url);
  }

  return {
    show,
    subtitle,
    toggleFavorite,
    onPressTickets,
  };
}
