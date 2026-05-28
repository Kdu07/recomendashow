import { createNativeStackNavigator } from '@react-navigation/native-stack';
import { View, Text } from 'react-native';

import WelcomeScreen from './screens/welcome/WelcomeScreen';
import OnboardingScreen from './screens/onboarding/OnboardingScreen';
import ProfileScreen from './screens/ProfileScreen';
import EditInterestsScreen from './screens/EditInterestsScreen';
import NotificationsScreen from './screens/NotificationsScreen';
import InstitutionalScreen from './screens/InstitutionalScreen';

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
      <Stack.Screen name="Onboarding" component={OnboardingScreen} />
      <Stack.Screen name="Welcome" component={WelcomeScreen} />
      <Stack.Screen name="Home" component={HomePlaceholder} />
      <Stack.Screen name="Profile"  component={ProfileScreen}/>
      <Stack.Screen name="EditInterests"  component={EditInterestsScreen}/>
      <Stack.Screen name="Notifications"  component={NotificationsScreen}/>
      <Stack.Screen name="Institutional"  component={InstitutionalScreen}/>
    </Stack.Navigator>
  );
}