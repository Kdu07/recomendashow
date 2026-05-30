import { useEffect } from 'react';

const DURATION = 3500;

export function useWelcomeViewModel(onFinish) {
  useEffect(() => {
    const timer = setTimeout(() => onFinish(), DURATION);
    return () => clearTimeout(timer);
  }, []);
}
