import React from 'react';
import { View, ScrollView, StyleSheet, SafeAreaView } from 'react-native';

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

export default function ShowInfoScreen({ route }) {
  const showId = route?.params?.showId ?? '1';
  const { show, subtitle, toggleFavorite, onPressTickets } = useShowInfoViewModel(showId);

  return (
    <SafeAreaView style={styles.container}>
      <ScrollView
        style={styles.scroll}
        contentContainerStyle={styles.scrollContent}
        showsVerticalScrollIndicator={false}
      >
        <ShowHero heroImage={show.heroImage} />

        <ShowInfoHeader
          title={show.title}
          subtitle={subtitle}
          isFavorited={show.isFavorited}
          onToggleFavorite={toggleFavorite}
          onPressTickets={onPressTickets}
        />

        <ShowDetails location={show.location} />

        <ShowAbout about={show.about} />

        {show.cast && <ShowCast cast={show.cast} />}

        <ShowCreativeTeam creativeTeam={show.creativeTeam} />

        <ShowRecommendations recommendations={show.recommendations} />

        <View style={styles.scrollBottomPadding} />
      </ScrollView>
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
});
