import React from 'react';

import { useOnboardingViewModel } from '../../viewmodels/useOnboardingViewModel';
import OnboardingCategoriesView from './views/OnboardingCategoriesView';
import OnboardingLocationView from './views/OnboardingLocationView';
import OnboardingFrequencyView from './views/OnboardingFrequencyView';

export default function OnboardingScreen({ navigation }) {
  const vm = useOnboardingViewModel(() => {
    navigation.replace('Home');
  });

  if (vm.step === 0) {
    return (
      <OnboardingCategoriesView
        step={vm.step}
        totalSteps={vm.totalSteps}
        selectedCategories={vm.selectedCategories}
        onToggleCategory={vm.toggleCategory}
        onContinue={vm.goNext}
        onSkip={vm.skip}
      />
    );
  }

  if (vm.step === 1) {
    return (
      <OnboardingLocationView
        step={vm.step}
        totalSteps={vm.totalSteps}
        selectedCity={vm.selectedCity}
        onSelectCity={vm.selectCity}
        onContinue={vm.goNext}
        onSkip={vm.skip}
      />
    );
  }

  return (
    <OnboardingFrequencyView
      step={vm.step}
      totalSteps={vm.totalSteps}
      selectedFrequency={vm.selectedFrequency}
      onSelectFrequency={vm.selectFrequency}
      onContinue={vm.goNext}
      onSkip={vm.skip}
    />
  );
}
