import React from 'react';
import { View, Text, TouchableOpacity, StyleSheet } from 'react-native';

import { colors } from '../../../constants/colors';
import { typography } from '../../../constants/typography';
import { spacing, radius } from '../../../constants/spacing';

export default function ShowInfoHeader({
  title,
  subtitle,
  isFavorited,
  onToggleFavorite,
  onPressTickets,
  compact = false,
}) {
  return (
    <View style={[styles.container, compact && styles.containerCompact]}>
      <View style={styles.row}>
        <Text
          style={[styles.title, compact && styles.titleCompact]}
          numberOfLines={compact ? 1 : 2}
        >
          {title}
        </Text>

        <View style={styles.actions}>
          <TouchableOpacity
            style={styles.favoriteButton}
            onPress={onToggleFavorite}
            activeOpacity={0.8}
          >
            <Text style={styles.heartIcon}>{isFavorited ? '♥' : '♡'}</Text>
          </TouchableOpacity>

          <TouchableOpacity
            style={styles.ticketButton}
            onPress={onPressTickets}
            activeOpacity={0.8}
          >
            <Text style={styles.ticketLabel}>INGRESSOS</Text>
          </TouchableOpacity>
        </View>
      </View>

      <Text style={[styles.subtitle, compact && styles.subtitleCompact]}>
        {subtitle}
      </Text>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    paddingHorizontal: spacing.xl,
    paddingTop: spacing.lg,
    paddingBottom: spacing.sm,
    backgroundColor: colors.white,
    gap: spacing.xs,
  },

  containerCompact: {
    paddingTop: spacing.md,
    paddingBottom: spacing.md,
    borderBottomWidth: 1,
    borderBottomColor: colors.grayLight,
  },

  row: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    gap: spacing.sm,
  },

  title: {
    flex: 1,
    fontSize: typography.size.xl,
    fontWeight: typography.fontWeight.bold,
    color: colors.grayDark,
  },

  titleCompact: {
    fontSize: typography.size.md,
  },

  actions: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: spacing.sm,
  },

  favoriteButton: {
    width: 36,
    height: 36,
    borderRadius: radius.pill,
    backgroundColor: colors.purpleSoft,
    alignItems: 'center',
    justifyContent: 'center',
  },

  heartIcon: {
    fontSize: typography.size.md,
    color: colors.purplePrimary,
  },

  ticketButton: {
    backgroundColor: colors.purplePrimary,
    paddingHorizontal: spacing.lg,
    paddingVertical: spacing.sm,
    borderRadius: radius.md,
  },

  ticketLabel: {
    fontSize: typography.size.xs,
    fontWeight: typography.fontWeight.bold,
    color: colors.white,
    letterSpacing: 0.5,
  },

  subtitle: {
    fontSize: typography.size.sm,
    fontWeight: typography.fontWeight.regular,
    color: colors.grayText,
  },

  subtitleCompact: {
    fontSize: typography.size.xs,
  },
});
