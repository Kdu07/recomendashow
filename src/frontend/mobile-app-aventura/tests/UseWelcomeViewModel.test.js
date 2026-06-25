import { renderHook } from '@testing-library/react';

import { useWelcomeViewModel } from '../viewmodels/useWelcomeViewModel';

describe('useWelcomeViewModel', () => {
  beforeEach(() => {
    jest.useFakeTimers();
  });

  afterEach(() => {
    jest.useRealTimers();
  });

  test('does not call onFinish immediately', () => {
    const onFinish = jest.fn();
    renderHook(() => useWelcomeViewModel(onFinish));

    expect(onFinish).not.toHaveBeenCalled();
  });

  test('calls onFinish after the splash duration elapses', () => {
    const onFinish = jest.fn();
    renderHook(() => useWelcomeViewModel(onFinish));

    jest.advanceTimersByTime(3500);

    expect(onFinish).toHaveBeenCalledTimes(1);
  });

  test('does not call onFinish before the splash duration elapses', () => {
    const onFinish = jest.fn();
    renderHook(() => useWelcomeViewModel(onFinish));

    jest.advanceTimersByTime(3499);

    expect(onFinish).not.toHaveBeenCalled();
  });

  test('clears the timer on unmount, preventing onFinish from firing afterwards', () => {
    const onFinish = jest.fn();
    const { unmount } = renderHook(() => useWelcomeViewModel(onFinish));

    unmount();
    jest.advanceTimersByTime(3500);

    expect(onFinish).not.toHaveBeenCalled();
  });
});