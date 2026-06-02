import React, { useRef, useState } from 'react';
import { View, Text, StyleSheet, SafeAreaView, Animated, Platform, StatusBar, TouchableOpacity } from 'react-native';
import { Ionicons } from '@expo/vector-icons';

import { useShowInfoViewModel } from '../../viewmodels/useShowInfoViewModel';
import ShowHero from './components/ShowHero';
import ShowInfoHeader from './components/ShowInfoHeader';
import ShowDetails from './components/ShowDetails';
import ShowAbout from './components/ShowAbout';
import ShowCast from './components/ShowCast';
import ShowTicketCard from './components/ShowTicketCard';
import ShowCreativeTeam from './components/ShowCreativeTeam';
import ShowRecommendations from './components/ShowRecommendations';
import BottomTabBar from '../../components/BottomTabBar';
import SystemStatus from '../../components/SystemStatus';

import { colors } from '../../constants/colors';
import { radius, spacing } from '../../constants/spacing';

const tabToRoute = {
  explore: 'Home',
  search: 'Search',
  wishlist: 'Wishlist',
  profile: 'Profile',
};

const HERO_HEIGHT = 260;
const INLINE_FADE_START = 150;
const INLINE_FADE_END = 210;
const STICKY_FADE_START = 215;
const STICKY_FADE_END = 260;

export default function ShowInfoScreen({ route, navigation }) {
  const [systemStatus, setSystemStatus] = useState(null);
  const showId = route?.params?.showId ?? '1';
  const topInset = Platform.OS === 'android' ? StatusBar.currentHeight || 0 : 0;
  const { show, error, subtitle, toggleFavorite, onPressTickets, onPressVenue } = useShowInfoViewModel(showId);

  function handleBackPress() {
    if (navigation.canGoBack()) {
      navigation.goBack();
      return;
    }

    navigation.navigate('MainTabs', { screen: 'Home' });
  }

  if (error) {
    return (
      <SafeAreaView style={styles.container}>
        <TouchableOpacity
          style={[styles.backButton, { top: topInset + spacing.xxxl }]}
          onPress={handleBackPress}
          activeOpacity={0.75}
        >
          <Ionicons name="chevron-back" size={22} color={colors.grayDark} />
        </TouchableOpacity>

        <View style={styles.errorContainer}>
          <Text style={styles.errorText}>Espetáculo não encontrado.</Text>
        </View>
      </SafeAreaView>
    );
  }

  const scrollY = useRef(new Animated.Value(0)).current;

  const heroOpacity = scrollY.interpolate({
    inputRange: [0, HERO_HEIGHT * 0.75],
    outputRange: [1, 0],
    extrapolate: 'clamp',
  });

  const inlineHeaderOpacity = scrollY.interpolate({
    inputRange: [INLINE_FADE_START, INLINE_FADE_END],
    outputRange: [1, 0],
    extrapolate: 'clamp',
  });

  const stickyOpacity = scrollY.interpolate({
    inputRange: [STICKY_FADE_START, STICKY_FADE_END],
    outputRange: [0, 1],
    extrapolate: 'clamp',
  });

  function handlePressVenueWithStatus() {
    onPressVenue();
    setSystemStatus({
      type: 'info',
      message: 'Abrindo informações do local.',
    });
  }

  async function handleToggleFavoriteWithStatus() {
    const result = await toggleFavorite();

    setSystemStatus({
      type: result?.added ? 'success' : 'info',
      message: result?.added
        ? 'Espetáculo adicionado à wishlist.'
        : 'Espetáculo removido da wishlist.',
    });
  }

  function handlePressTicketsWithStatus() {
    onPressTickets();
    setSystemStatus({
      type: 'info',
      message: 'Abrindo página de ingressos.',
    });
  }

  const headerProps = {
    title: show.title,
    subtitle,
    isFavorited: show.isFavorited,
    onToggleFavorite: handleToggleFavoriteWithStatus,
    onPressTickets: handlePressTicketsWithStatus,
  };

  return (
    <SafeAreaView style={styles.container}>
      <Animated.ScrollView
        style={styles.scroll}
        contentContainerStyle={styles.scrollContent}
        showsVerticalScrollIndicator={false}
        onScroll={Animated.event(
          [{ nativeEvent: { contentOffset: { y: scrollY } } }],
          { useNativeDriver: true }
        )}
        scrollEventThrottle={16}
      >
        <Animated.View style={{ opacity: heroOpacity }}>
          <ShowHero heroImage={show.heroImage} />
        </Animated.View>

        <Animated.View style={{ opacity: inlineHeaderOpacity }}>
          <ShowInfoHeader {...headerProps} />
        </Animated.View>

        <ShowAbout about={show.about} />

        <ShowDetails location={show.location} onPressVenue={handlePressVenueWithStatus} />

        <ShowTicketCard onPress={handlePressTicketsWithStatus} />

        {show.cast && <ShowCast cast={show.cast} />}

        <ShowCreativeTeam creativeTeam={show.creativeTeam} />

        <ShowRecommendations recommendations={show.recommendations} />

        <View style={styles.scrollBottomPadding} />
      </Animated.ScrollView>

      <Animated.View
        style={[
          styles.stickyHeader,
          {
            opacity: stickyOpacity,
            paddingTop: topInset + spacing.xxxl,
          },
        ]}
        pointerEvents="box-none"
      >
        <ShowInfoHeader {...headerProps} compact />
      </Animated.View>

      <TouchableOpacity
        style={[styles.backButton, { top: topInset + spacing.xxxl }]}
        onPress={handleBackPress}
        activeOpacity={0.75}
      >
        <Ionicons name="chevron-back" size={22} color={colors.grayDark} />
      </TouchableOpacity>

      <BottomTabBar
        activeTab="explore"
        onTabPress={(tab) => {
          const routeName = tabToRoute[tab];
          if (routeName) {
            navigation.navigate('MainTabs', { screen: routeName });
          }
        }}
      />

      <SystemStatus
        message={systemStatus?.message}
        type={systemStatus?.type}
        onDismiss={() => setSystemStatus(null)}
      />
    </SafeAreaView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: colors.white,
  },

  scroll: {
    flex: 1,
  },

  scrollContent: {
    paddingTop: spacing.md,
    paddingBottom: spacing.sm,
  },

  scrollBottomPadding: {
    height: 100,
  },

  stickyHeader: {
    position: 'absolute',
    top: 0,
    left: 0,
    right: 0,
    paddingLeft: 52,
    backgroundColor: colors.white,
  },

  backButton: {
    position: 'absolute',
    left: spacing.md,
    zIndex: 10,
    width: 40,
    height: 40,
    borderRadius: radius.pill,
    backgroundColor: colors.white,
    borderWidth: 1,
    borderColor: colors.grayLight,
    alignItems: 'center',
    justifyContent: 'center',
    shadowColor: colors.black,
    shadowOpacity: 0.08,
    shadowRadius: 8,
    shadowOffset: {
      width: 0,
      height: 3,
    },
    elevation: 4,
  },

  errorContainer: {
    flex: 1,
    alignItems: 'center',
    justifyContent: 'center',
  },

  errorText: {
    fontSize: 16,
    color: '#666',
  },
});
