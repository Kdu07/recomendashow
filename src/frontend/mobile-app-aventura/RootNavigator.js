import { createNativeStackNavigator } from '@react-navigation/native-stack';
import { View, Text } from 'react-native';

import WelcomeScreen from './screens/welcome/WelcomeScreen';
import OnboardingScreen from './screens/onboarding/OnboardingScreen';

const Stack = createNativeStackNavigator();

function HomePlaceholder() {
  return (
    <View style={{ flex: 1, justifyContent: 'center', alignItems: 'center' }}>
      <Text>Onboarding concluído! 🎉</Text>
    </View>
  );
}

export default function RootNavigator() {
  return (
    <Stack.Navigator screenOptions={{ headerShown: false }}>
      <Stack.Screen name="Welcome" component={WelcomeScreen} />
      <Stack.Screen name="Onboarding" component={OnboardingScreen} />
      <Stack.Screen name="Home" component={HomePlaceholder} />
    </Stack.Navigator>
  );
}