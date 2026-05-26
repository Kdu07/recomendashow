import React, { useRef, useEffect } from 'react';
import { Animated } from 'react-native';

import { useOnboardingViewModel } from '../../viewmodels/useOnboardingViewModel';
import OnboardingCategoriesView from './views/OnboardingCategoriesView';
import OnboardingLocationView from './views/OnboardingLocationView';
import OnboardingFrequencyView from './views/OnboardingFrequencyView';

export default function OnboardingScreen({ navigation }) {
  const translateY = useRef(new Animated.Value(50)).current;
  const opacity = useRef(new Animated.Value(0)).current;

  const vm = useOnboardingViewModel(() => {
    navigation.replace('Home');
  });

  useEffect(() => {
    translateY.setValue(50);
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

  function animateTransition(action) {
    Animated.parallel([
      Animated.timing(translateY, {
        toValue: -40,
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
    onContinue: () => animateTransition(vm.goNext),
    onSkip: () => animateTransition(vm.skip),
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
