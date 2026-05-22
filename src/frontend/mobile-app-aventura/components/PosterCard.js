import React from 'react';
import { TouchableOpacity, Image, Text, StyleSheet, View } from 'react-native';

import { colors } from '../constants/colors';
import { typography } from '../constants/typography';
import { radius, spacing } from '../constants/spacing';

export default function PosterCard({
  title,
  subtitle,
  image,
  onPress,
  width = 104,
  imageHeight = 146,
}) {
  return (
    <TouchableOpacity
      style={[styles.container, { width }]}
      onPress={onPress}
      activeOpacity={0.85}
    >
      <View style={[styles.imageWrapper, { height: imageHeight }]}>
        <Image source={image} style={styles.image} resizeMode="cover" />
      </View>

      <Text style={styles.title} numberOfLines={1}>
        {title}
      </Text>

      {subtitle && (
        <Text style={styles.subtitle} numberOfLines={1}>
          {subtitle}
        </Text>
      )}
    </TouchableOpacity>
  );
}

const styles = StyleSheet.create({
  container: {
    gap: spacing.xs,
  },

  imageWrapper: {
    width: '100%',
    borderRadius: radius.md,
    overflow: 'hidden',
    backgroundColor: colors.purpleSoft,
  },

  image: {
    width: '100%',
    height: '100%',
  },

  title: {
    fontSize: typography.size.xs,
    fontWeight: typography.fontWeight.medium,
    color: colors.grayDark,
  },

  subtitle: {
    fontSize: typography.size.xs,
    color: colors.peachBurnt,
  },
});