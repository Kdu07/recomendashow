import React from 'react';
import { View, Text, StyleSheet, TouchableOpacity } from 'react-native';

import { colors } from '../constants/colors';
import { typography } from '../constants/typography';
import { spacing, radius } from '../constants/spacing';

export default function EmptyState({
  icon = '💡',
  title,
  description,
  actionLabel,
  onActionPress,
}) {
  return (
    <View style={styles.container}>
      <View style={styles.iconContainer}>
        <Text style={styles.icon}>{icon}</Text>
      </View>

      <Text style={styles.title}>{title}</Text>

      <Text style={styles.description}>{description}</Text>

      {actionLabel && (
        <TouchableOpacity
          style={styles.actionButton}
          onPress={onActionPress}
          activeOpacity={0.75}
        >
          <Text style={styles.actionText}>{actionLabel}</Text>
        </TouchableOpacity>
      )}
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    width: '100%',
    alignItems: 'center',
    justifyContent: 'center',
    paddingVertical: spacing.xxl,
    paddingHorizontal: spacing.lg,
    backgroundColor: colors.white,
  },

  iconContainer: {
    width: 72,
    height: 72,
    borderRadius: radius.pill,
    backgroundColor: colors.whiteSoft,
    alignItems: 'center',
    justifyContent: 'center',
    marginBottom: spacing.md,
  },

  icon: {
    fontSize: 38,
  },

  title: {
    fontSize: typography.size.md,
    fontWeight: typography.fontWeight.bold,
    color: colors.grayDark,
    textAlign: 'center',
    marginBottom: spacing.sm,
  },

  description: {
    maxWidth: 280,
    fontSize: typography.size.sm,
    lineHeight: typography.lineHeight.sm,
    color: colors.grayText,
    textAlign: 'center',
    marginBottom: spacing.md,
  },

  actionButton: {
    paddingVertical: spacing.xs,
    paddingHorizontal: spacing.sm,
  },

  actionText: {
    fontSize: typography.size.sm,
    fontWeight: typography.fontWeight.semiBold,
    color: colors.purplePrimary,
    textDecorationLine: 'underline',
  },
});