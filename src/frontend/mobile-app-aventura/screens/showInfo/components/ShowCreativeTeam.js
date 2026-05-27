import React from 'react';
import { View, Text, StyleSheet } from 'react-native';

import { colors } from '../../../constants/colors';
import { typography } from '../../../constants/typography';
import { spacing, radius } from '../../../constants/spacing';

export default function ShowCreativeTeam({ creativeTeam }) {
  return (
    <View style={styles.container}>
      <View style={styles.card}>
        <Text style={styles.sectionLabel}>{creativeTeam.sectionLabel}</Text>

        <View style={styles.list}>
          {creativeTeam.members.map((member, index) => (
            <Text key={index} style={styles.member}>
              <Text style={styles.role}>{member.role}: </Text>
              <Text style={styles.name}>{member.name}</Text>
            </Text>
          ))}
        </View>
      </View>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    paddingHorizontal: spacing.xl,
    paddingTop: spacing.sm,
  },

  card: {
    backgroundColor: colors.grayLight,
    borderRadius: radius.md,
    padding: spacing.lg,
    gap: spacing.sm,
  },

  sectionLabel: {
    fontSize: typography.size.xs,
    fontWeight: typography.fontWeight.semiBold,
    color: colors.grayText,
    letterSpacing: 0.5,
  },

  list: {
    gap: spacing.xs,
  },

  member: {
    fontSize: typography.size.sm,
    lineHeight: typography.lineHeight.sm,
  },

  role: {
    fontWeight: typography.fontWeight.semiBold,
    color: colors.grayDark,
  },

  name: {
    fontWeight: typography.fontWeight.regular,
    color: colors.grayDark,
  },
});
