import React from 'react';
import { View, Text, Image, StyleSheet } from 'react-native';

import { colors } from '../../../constants/colors';
import { typography } from '../../../constants/typography';
import { spacing, radius } from '../../../constants/spacing';

function DetailRow({ icon, label, children }) {
  return (
    <View style={styles.row}>
      <Text style={styles.icon}>{icon}</Text>
      <View style={styles.rowContent}>
        <Text style={styles.label}>{label}</Text>
        {children}
      </View>
    </View>
  );
}

export default function ShowDetails({ date, location }) {
  return (
    <View style={styles.container}>
      <DetailRow icon="📅" label={date.label}>
        <Text style={styles.primary}>{date.status}</Text>
        <Text style={styles.secondary}>{date.detail}</Text>
      </DetailRow>

      <View style={styles.divider} />

      <DetailRow icon="📍" label={location.label}>
        <Text style={styles.primary}>{location.venue}</Text>
        <Text style={styles.secondary}>{location.address}</Text>
        <Text style={styles.secondary}>{location.city}</Text>
      </DetailRow>

      <Image
        source={location.mapImage}
        style={styles.map}
        resizeMode="cover"
      />
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    paddingHorizontal: spacing.xl,
    paddingTop: spacing.lg,
    gap: spacing.lg,
  },

  row: {
    flexDirection: 'row',
    gap: spacing.md,
    alignItems: 'flex-start',
  },

  icon: {
    fontSize: typography.size.lg,
    marginTop: 2,
  },

  rowContent: {
    flex: 1,
    gap: 2,
  },

  label: {
    fontSize: typography.size.xs,
    fontWeight: typography.fontWeight.semiBold,
    color: colors.grayText,
    letterSpacing: 0.5,
    marginBottom: spacing.xs,
  },

  primary: {
    fontSize: typography.size.md,
    fontWeight: typography.fontWeight.medium,
    color: colors.grayDark,
  },

  secondary: {
    fontSize: typography.size.sm,
    fontWeight: typography.fontWeight.regular,
    color: colors.grayText,
  },

  divider: {
    height: 1,
    backgroundColor: colors.grayLight,
  },

  map: {
    width: '100%',
    height: 160,
    borderRadius: radius.md,
  },
});
