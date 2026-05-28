import { useState } from 'react';
import { MOCK_SHOWS } from '../models/showInfoData';

export function useShowInfoViewModel(showId = '1') {
  const [show, setShow] = useState(MOCK_SHOWS[showId]);

  function toggleFavorite() {
    setShow((prev) => ({ ...prev, isFavorited: !prev.isFavorited }));
  }

  function onPressTickets() {
    console.warn('[ShowInfo] onPressTickets not implemented yet');
  }

  return {
    show,
    toggleFavorite,
    onPressTickets,
  };
}
