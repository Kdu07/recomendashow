import { useState } from 'react';
import { MOCK_SHOW } from '../models/showInfoData';

export function useShowInfoViewModel() {
  const [show, setShow] = useState(MOCK_SHOW);

  function toggleFavorite() {
    setShow((prev) => ({ ...prev, isFavorited: !prev.isFavorited }));
  }

  return {
    show,
    toggleFavorite,
  };
}
