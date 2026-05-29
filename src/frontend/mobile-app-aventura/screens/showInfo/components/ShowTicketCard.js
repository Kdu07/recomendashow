import React from 'react';
import { TouchableOpacity, Text, StyleSheet } from 'react-native';

import { colors } from '../../../constants/colors';
import { typography } from '../../../constants/typography';
import { spacing, radius } from '../../../constants/spacing';

export default function ShowTicketCard({ onPress }) {
  return (
    <TouchableOpacity style={styles.card} onPress={onPress} activeOpacity={0.8}>
      <Text style={styles.label}>COMPRAR INGRESSO</Text>
    </TouchableOpacity>
  );
}

const styles = StyleSheet.create({
  card: {
    marginHorizontal: spacing.xl,
    marginTop: 30,
    marginBottom: spacing.md,
    backgroundColor: colors.purplePrimary,
    borderRadius: radius.md,
    padding: spacing.lg,
  },

  label: {
    fontSize: typography.size.xs,
    fontWeight: typography.fontWeight.bold,
    color: colors.white,
    letterSpacing: 0.5,
    textAlign: 'center',
  },
});
