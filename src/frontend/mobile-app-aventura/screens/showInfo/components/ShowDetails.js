import React from 'react';
import { View, Text, Image, StyleSheet } from 'react-native';

import { colors } from '../../../constants/colors';
import { typography } from '../../../constants/typography';
import { spacing, radius } from '../../../constants/spacing';

export default function ShowDetails({ location }) {
  return (
    <View style={styles.container}>
      <Text style={styles.sectionLabel}>{location.label}</Text>

      <Image
        source={location.mapImage}
        style={styles.map}
        resizeMode="cover"
      />

      <View style={styles.venueInfo}>
        <Text style={styles.venue}>{location.venue}</Text>
        <Text style={styles.address}>{location.address}</Text>
      </View>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    paddingHorizontal: spacing.xl,
    paddingTop: spacing.xl,
    gap: spacing.md,
  },

  sectionLabel: {
    fontSize: 17,
    fontWeight: typography.fontWeight.bold,
    color: colors.grayDark,
  },

  map: {
    width: '100%',
    height: 160,
    borderRadius: radius.md,
  },

  venueInfo: {
    gap: 2,
  },

  venue: {
    fontSize: typography.size.sm,
    fontWeight: typography.fontWeight.bold,
    color: colors.grayDark,
  },

  address: {
    fontSize: typography.size.sm,
    fontWeight: typography.fontWeight.regular,
    color: colors.grayText,
  },
});
