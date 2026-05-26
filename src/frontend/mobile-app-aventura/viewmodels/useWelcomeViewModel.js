import { useEffect } from 'react';

const DURATION = 5000;

export function useWelcomeViewModel(onFinish) {
  useEffect(() => {
    const timer = setTimeout(() => onFinish(), DURATION);
    return () => clearTimeout(timer);
  }, []);
}
