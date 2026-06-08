import { createNativeStackNavigator } from '@react-navigation/native-stack';
import { createBottomTabNavigator } from '@react-navigation/bottom-tabs';

import SplashScreen from './screens/SplashScreen';
import WelcomeScreen from './screens/welcome/WelcomeScreen';
import OnboardingScreen from './screens/onboarding/OnboardingScreen';
import AuthScreen from './screens/AuthScreen';
import HomeScreen from './screens/HomeScreen';
import SearchScreen from './screens/SearchScreen';
import WishlistScreen from './screens/WishlistScreen';
import ProfileScreen from './screens/ProfileScreen';
import EditProfileScreen from './screens/EditProfileScreen';
import EditInterestsScreen from './screens/EditInterestsScreen';
import NotificationsScreen from './screens/NotificationsScreen';
import InstitutionalScreen from './screens/InstitutionalScreen';
import ShowInfoScreen from './screens/showInfo/ShowInfoScreen';

import BottomTabBar from './components/BottomTabBar';

const Stack = createNativeStackNavigator();
const Tab = createBottomTabNavigator();

const routeToTab = {
  Home: 'explore',
  Search: 'search',
  Wishlist: 'wishlist',
  Profile: 'profile',
};

const tabToRoute = {
  explore: 'Home',
  search: 'Search',
  wishlist: 'Wishlist',
  profile: 'Profile',
};

function MainTabs() {
  return (
    <Tab.Navigator
      screenOptions={{
        headerShown: false,
        lazy: false,
      }}
      tabBar={({ state, navigation }) => {
        const currentRouteName = state.routes[state.index].name;
        const activeTab = routeToTab[currentRouteName] || 'explore';

        return (
          <BottomTabBar
            activeTab={activeTab}
            onTabPress={(tab) => {
              const routeName = tabToRoute[tab];

              if (routeName) {
                navigation.navigate(routeName);
              }
            }}
          />
        );
      }}
    >
      <Tab.Screen name="Home" component={HomeScreen} />
      <Tab.Screen name="Search" component={SearchScreen} />
      <Tab.Screen name="Wishlist" component={WishlistScreen} />
      <Tab.Screen name="Profile" component={ProfileScreen} />
    </Tab.Navigator>
  );
}

export default function RootNavigator() {
  return (
    <Stack.Navigator
      initialRouteName="AppSplash"
      screenOptions={{
        headerShown: false,
        animation: 'fade',
      }}
    >
      <Stack.Screen name="AppSplash" component={SplashScreen} />
      <Stack.Screen name="Auth" component={AuthScreen} />
      <Stack.Screen name="Onboarding" component={OnboardingScreen} />
      <Stack.Screen name="Splash" component={WelcomeScreen} />
      <Stack.Screen name="EditProfile" component={EditProfileScreen} />
      <Stack.Screen name="EditInterests" component={EditInterestsScreen} />
      <Stack.Screen name="Notifications" component={NotificationsScreen} />
      <Stack.Screen name="Institutional" component={InstitutionalScreen} />
      <Stack.Screen name="MainTabs" component={MainTabs} />
      <Stack.Screen name="ShowInfo" component={ShowInfoScreen} />
    </Stack.Navigator>
  );
}
