import React, { useRef } from 'react';
import { View, StyleSheet, SafeAreaView, Animated } from 'react-native';

import { useShowInfoViewModel } from '../../viewmodels/useShowInfoViewModel';
import ShowHero from './components/ShowHero';
import ShowInfoHeader from './components/ShowInfoHeader';
import ShowDetails from './components/ShowDetails';
import ShowAbout from './components/ShowAbout';
import ShowCast from './components/ShowCast';
import ShowCreativeTeam from './components/ShowCreativeTeam';
import ShowRecommendations from './components/ShowRecommendations';

import { colors } from '../../constants/colors';
import { spacing } from '../../constants/spacing';

const HERO_HEIGHT = 260;
const STICKY_FADE_START = 200;
const STICKY_FADE_END = 260;

export default function ShowInfoScreen({ route }) {
  const showId = route?.params?.showId ?? '1';
  const { show, subtitle, toggleFavorite, onPressTickets } = useShowInfoViewModel(showId);

  const scrollY = useRef(new Animated.Value(0)).current;

  const heroOpacity = scrollY.interpolate({
    inputRange: [0, HERO_HEIGHT * 0.75],
    outputRange: [1, 0],
    extrapolate: 'clamp',
  });

  const stickyOpacity = scrollY.interpolate({
    inputRange: [STICKY_FADE_START, STICKY_FADE_END],
    outputRange: [0, 1],
    extrapolate: 'clamp',
  });

  const headerProps = {
    title: show.title,
    subtitle,
    isFavorited: show.isFavorited,
    onToggleFavorite: toggleFavorite,
    onPressTickets,
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

        <ShowInfoHeader {...headerProps} />

        <ShowDetails location={show.location} />

        <ShowAbout about={show.about} />

        {show.cast && <ShowCast cast={show.cast} />}

        <ShowCreativeTeam creativeTeam={show.creativeTeam} />

        <ShowRecommendations recommendations={show.recommendations} />

        <View style={styles.scrollBottomPadding} />
      </Animated.ScrollView>

      <Animated.View
        style={[styles.stickyHeader, { opacity: stickyOpacity }]}
        pointerEvents="box-none"
      >
        <ShowInfoHeader {...headerProps} compact />
      </Animated.View>
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
    paddingBottom: spacing.sm,
  },

  scrollBottomPadding: {
    height: spacing.xl,
  },

  stickyHeader: {
    position: 'absolute',
    top: 0,
    left: 0,
    right: 0,
  },
});
