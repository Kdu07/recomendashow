import React from 'react';

import { NavigationContainer } from '@react-navigation/native';

import { createNativeStackNavigator } from '@react-navigation/native-stack';

import HomeScreen from './screens/HomeScreen';
import ProfileScreen from './screens/ProfileScreen';
import EditInterestsScreen from './screens/EditInterestsScreen';
import NotificationsScreen from './screens/NotificationsScreen';
import InstitutionalScreen from './screens/InstitutionalScreen';

const Stack = createNativeStackNavigator();

export default function AppNavigator() {
  return (
    <NavigationContainer>
      <Stack.Navigator
        screenOptions={{
          headerShown: false,
        }}
      >
        <Stack.Screen
          name="Home"
          component={HomeScreen}
        />

        <Stack.Screen
          name="Profile"
          component={ProfileScreen}
        />

        <Stack.Screen
          name="EditInterests"
          component={EditInterestsScreen}
        />

        <Stack.Screen
          name="Notifications"
          component={NotificationsScreen}
        />

        <Stack.Screen
          name="Institutional"
          component={InstitutionalScreen}
        />
      </Stack.Navigator>
    </NavigationContainer>
  );
}