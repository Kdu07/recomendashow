import React, { useRef, useEffect } from 'react';
import { Animated } from 'react-native';

import { useOnboardingViewModel } from '../../viewmodels/useOnboardingViewModel';
import OnboardingCategoriesView from './views/OnboardingCategoriesView';
import OnboardingLocationView from './views/OnboardingLocationView';
import OnboardingFrequencyView from './views/OnboardingFrequencyView';

export default function OnboardingScreen({ navigation }) {
  const translateY = useRef(new Animated.Value(50)).current;
  const opacity = useRef(new Animated.Value(0)).current;
  const enterDirection = useRef('forward');

  const vm = useOnboardingViewModel(() => {
    navigation.replace('Splash');
  });

  useEffect(() => {
    const startY = enterDirection.current === 'forward' ? 50 : -50;
    translateY.setValue(startY);
    opacity.setValue(0);
    Animated.parallel([
      Animated.timing(translateY, {
        toValue: 0,
        duration: 320,
        useNativeDriver: true,
      }),
      Animated.timing(opacity, {
        toValue: 1,
        duration: 260,
        useNativeDriver: true,
      }),
    ]).start();
  }, [vm.step]);

  function animateTransition(action, direction = 'forward') {
    enterDirection.current = direction;
    const exitY = direction === 'forward' ? -40 : 40;
    Animated.parallel([
      Animated.timing(translateY, {
        toValue: exitY,
        duration: 200,
        useNativeDriver: true,
      }),
      Animated.timing(opacity, {
        toValue: 0,
        duration: 160,
        useNativeDriver: true,
      }),
    ]).start(() => {
      action();
    });
  }

  const animatedStyle = {
    opacity,
    transform: [{ translateY }],
  };

  const sharedProps = {
    step: vm.step,
    totalSteps: vm.totalSteps,
    animatedStyle,
    onContinue: () => animateTransition(vm.goNext, 'forward'),
    onSkip: () => animateTransition(vm.skip, 'forward'),
    onBack: () => animateTransition(vm.goBack, 'back'),
  };

  if (vm.step === 0) {
    return (
      <OnboardingCategoriesView
        {...sharedProps}
        selectedCategories={vm.selectedCategories}
        onToggleCategory={vm.toggleCategory}
      />
    );
  }

  if (vm.step === 1) {
    return (
      <OnboardingLocationView
        {...sharedProps}
        selectedCity={vm.selectedCity}
        onSelectCity={vm.selectCity}
      />
    );
  }

  return (
    <OnboardingFrequencyView
      {...sharedProps}
      selectedFrequency={vm.selectedFrequency}
      onSelectFrequency={vm.selectFrequency}
    />
  );
}
