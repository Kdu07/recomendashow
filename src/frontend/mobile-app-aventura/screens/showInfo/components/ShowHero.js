import React from 'react';
import { View, Image, TouchableOpacity, Text, StyleSheet } from 'react-native';

import { colors } from '../../../constants/colors';
import { spacing } from '../../../constants/spacing';
import { typography } from '../../../constants/typography';

export default function ShowHero({ heroImage, isFavorited, onToggleFavorite }) {
  return (
    <View style={styles.container}>
      <Image source={heroImage} style={styles.image} resizeMode="cover" />

      <TouchableOpacity
        style={styles.favoriteButton}
        onPress={onToggleFavorite}
        activeOpacity={0.8}
      >
        <Text style={[styles.heartIcon, isFavorited && styles.heartIconFilled]}>
          {isFavorited ? '♥' : '♡'}
        </Text>
      </TouchableOpacity>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    width: '100%',
    height: 260,
  },

  image: {
    width: '100%',
    height: '100%',
  },

  favoriteButton: {
    position: 'absolute',
    top: spacing.lg,
    right: spacing.lg,
    width: 40,
    height: 40,
    borderRadius: 20,
    backgroundColor: colors.white,
    alignItems: 'center',
    justifyContent: 'center',
  },

  heartIcon: {
    fontSize: typography.size.lg,
    color: colors.purplePrimary,
  },

  heartIconFilled: {
    color: colors.purplePrimary,
  },
});
