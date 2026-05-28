import React from 'react';
import { View, Text, ScrollView, TouchableOpacity, StyleSheet, SafeAreaView } from 'react-native';

import { useShowInfoViewModel } from '../../viewmodels/useShowInfoViewModel';
import ShowHero from './components/ShowHero';
import ShowDetails from './components/ShowDetails';
import ShowAbout from './components/ShowAbout';
import ShowCast from './components/ShowCast';
import ShowCreativeTeam from './components/ShowCreativeTeam';
import ShowRecommendations from './components/ShowRecommendations';

import { colors } from '../../constants/colors';
import { typography } from '../../constants/typography';
import { spacing, radius } from '../../constants/spacing';

export default function ShowInfoScreen() {
  const { show, toggleFavorite, onPressTickets } = useShowInfoViewModel();

  return (
    <SafeAreaView style={styles.container}>
      <ScrollView
        style={styles.scroll}
        contentContainerStyle={styles.scrollContent}
        showsVerticalScrollIndicator={false}
      >
        <ShowHero
          heroImage={show.heroImage}
          isFavorited={show.isFavorited}
          onToggleFavorite={toggleFavorite}
        />

        <View style={styles.titleContainer}>
          <Text style={styles.title}>{show.title}</Text>
        </View>

        <ShowDetails date={show.date} location={show.location} />

        <ShowAbout about={show.about} />

        <ShowCast cast={show.cast} />

        <ShowCreativeTeam creativeTeam={show.creativeTeam} />

        <ShowRecommendations recommendations={show.recommendations} />

        <View style={styles.scrollBottomPadding} />
      </ScrollView>

      <View style={styles.ticketBar}>
        <View style={styles.priceContainer}>
          <Text style={styles.priceLabel}>{show.ticket.priceLabel}</Text>
          <Text style={styles.price}>{show.ticket.price}</Text>
        </View>

        <TouchableOpacity
          style={styles.ticketButton}
          activeOpacity={0.8}
          onPress={onPressTickets}
        >
          <Text style={styles.ticketButtonLabel}>{show.ticket.buttonLabel}</Text>
        </TouchableOpacity>
      </View>
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

  titleContainer: {
    paddingHorizontal: spacing.xl,
    paddingTop: spacing.lg,
    paddingBottom: spacing.sm,
  },

  title: {
    fontSize: typography.size.xl,
    fontWeight: typography.fontWeight.bold,
    color: colors.grayDark,
  },

  scrollBottomPadding: {
    height: spacing.xl,
  },

  ticketBar: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    paddingHorizontal: spacing.xl,
    paddingVertical: spacing.md,
    backgroundColor: colors.white,
    borderTopWidth: 1,
    borderTopColor: colors.grayLight,
  },

  priceContainer: {
    gap: 2,
  },

  priceLabel: {
    fontSize: typography.size.xs,
    fontWeight: typography.fontWeight.regular,
    color: colors.grayText,
  },

  price: {
    fontSize: typography.size.lg,
    fontWeight: typography.fontWeight.bold,
    color: colors.purplePrimary,
  },

  ticketButton: {
    backgroundColor: colors.purplePrimary,
    paddingHorizontal: spacing.xl,
    paddingVertical: spacing.md,
    borderRadius: radius.md,
  },

  ticketButtonLabel: {
    fontSize: typography.size.sm,
    fontWeight: typography.fontWeight.bold,
    color: colors.white,
    letterSpacing: 0.5,
  },
});
