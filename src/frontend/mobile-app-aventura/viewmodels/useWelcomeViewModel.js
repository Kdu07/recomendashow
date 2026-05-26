import { useState, useEffect } from 'react';

const TOTAL_DOTS = 3;
const DURATION = 5000;

export function useWelcomeViewModel(onFinish) {
  const [activeDot, setActiveDot] = useState(0);

  useEffect(() => {
    const interval = DURATION / TOTAL_DOTS;

    const timers = [
      setTimeout(() => setActiveDot(1), interval),
      setTimeout(() => setActiveDot(2), interval * 2),
      setTimeout(() => onFinish(), DURATION),
    ];

    return () => timers.forEach(clearTimeout);
  }, []);

  return { activeDot, totalDots: TOTAL_DOTS };
}
